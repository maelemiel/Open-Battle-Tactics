namespace UnityEngine
{
	internal class SendMouseEvents
	{
		private struct HitInfo
		{
			public GameObject target;

			public Camera camera;

			public void SendMessage(string name)
			{
				target.SendMessage(name, null, SendMessageOptions.DontRequireReceiver);
			}

			public static bool Compare(HitInfo lhs, HitInfo rhs)
			{
				return lhs.target == rhs.target && lhs.camera == rhs.camera;
			}

			public static implicit operator bool(HitInfo exists)
			{
				return exists.target != null && exists.camera != null;
			}
		}

		private const int m_HitIndexGUI = 0;

		private const int m_HitIndexPhysics3D = 1;

		private const int m_HitIndexPhysics2D = 2;

		private static readonly HitInfo[] m_LastHit = new HitInfo[3]
		{
			default(HitInfo),
			default(HitInfo),
			default(HitInfo)
		};

		private static readonly HitInfo[] m_MouseDownHit = new HitInfo[3]
		{
			default(HitInfo),
			default(HitInfo),
			default(HitInfo)
		};

		private static readonly HitInfo[] m_CurrentHit = new HitInfo[3]
		{
			default(HitInfo),
			default(HitInfo),
			default(HitInfo)
		};

		private static readonly RaycastHit2D[] m_MouseRayHits2D = new RaycastHit2D[1] { default(RaycastHit2D) };

		private static Camera[] m_Cameras;

		[NotRenamed]
		private static void DoSendMouseEvents(int mouseUsed, int skipRTCameras)
		{
			Vector3 mousePosition = Input.mousePosition;
			int allCamerasCount = Camera.allCamerasCount;
			if (m_Cameras == null || m_Cameras.Length != allCamerasCount)
			{
				m_Cameras = new Camera[allCamerasCount];
			}
			Camera.GetAllCameras(m_Cameras);
			for (int i = 0; i < m_CurrentHit.Length; i++)
			{
				m_CurrentHit[i] = default(HitInfo);
			}
			if (mouseUsed == 0)
			{
				Camera[] cameras = m_Cameras;
				foreach (Camera camera in cameras)
				{
					if ((skipRTCameras != 0 && camera.targetTexture != null) || !camera.pixelRect.Contains(mousePosition))
					{
						continue;
					}
					GUILayer gUILayer = (GUILayer)camera.GetComponent(typeof(GUILayer));
					if ((bool)gUILayer)
					{
						GUIElement gUIElement = gUILayer.HitTest(mousePosition);
						if ((bool)gUIElement)
						{
							m_CurrentHit[0].target = gUIElement.gameObject;
							m_CurrentHit[0].camera = camera;
						}
						else
						{
							m_CurrentHit[0].target = null;
							m_CurrentHit[0].camera = null;
						}
					}
					if (camera.eventMask != 0)
					{
						Ray ray = camera.ScreenPointToRay(mousePosition);
						float z = ray.direction.z;
						float num = ((!Mathf.Approximately(0f, z)) ? Mathf.Abs((camera.farClipPlane - camera.nearClipPlane) / z) : float.PositiveInfinity);
						RaycastHit hitInfo;
						if (Physics.Raycast(ray, out hitInfo, num + 1f, camera.cullingMask & camera.eventMask & -5))
						{
							m_CurrentHit[1].camera = camera;
							m_CurrentHit[1].target = ((!hitInfo.rigidbody) ? hitInfo.collider.gameObject : hitInfo.rigidbody.gameObject);
						}
						else if (camera.clearFlags == CameraClearFlags.Skybox || camera.clearFlags == CameraClearFlags.Color)
						{
							m_CurrentHit[1].target = null;
							m_CurrentHit[1].camera = null;
						}
						if (Physics2D.GetRayIntersectionNonAlloc(ray, m_MouseRayHits2D, num, camera.cullingMask & camera.eventMask & -5) == 1)
						{
							m_CurrentHit[2].camera = camera;
							m_CurrentHit[2].target = ((!m_MouseRayHits2D[0].rigidbody) ? m_MouseRayHits2D[0].collider.gameObject : m_MouseRayHits2D[0].rigidbody.gameObject);
						}
						else if (camera.clearFlags == CameraClearFlags.Skybox || camera.clearFlags == CameraClearFlags.Color)
						{
							m_CurrentHit[2].target = null;
							m_CurrentHit[2].camera = null;
						}
					}
				}
			}
			for (int k = 0; k < m_CurrentHit.Length; k++)
			{
				SendEvents(k, m_CurrentHit[k]);
			}
		}

		private static void SendEvents(int i, HitInfo hit)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			bool mouseButton = Input.GetMouseButton(0);
			if (mouseButtonDown)
			{
				if ((bool)hit)
				{
					m_MouseDownHit[i] = hit;
					m_MouseDownHit[i].SendMessage("OnMouseDown");
				}
			}
			else if (!mouseButton)
			{
				if ((bool)m_MouseDownHit[i])
				{
					if (HitInfo.Compare(hit, m_MouseDownHit[i]))
					{
						m_MouseDownHit[i].SendMessage("OnMouseUpAsButton");
					}
					m_MouseDownHit[i].SendMessage("OnMouseUp");
					m_MouseDownHit[i] = default(HitInfo);
				}
			}
			else if ((bool)m_MouseDownHit[i])
			{
				m_MouseDownHit[i].SendMessage("OnMouseDrag");
			}
			if (HitInfo.Compare(hit, m_LastHit[i]))
			{
				if ((bool)hit)
				{
					hit.SendMessage("OnMouseOver");
				}
			}
			else
			{
				if ((bool)m_LastHit[i])
				{
					m_LastHit[i].SendMessage("OnMouseExit");
				}
				if ((bool)hit)
				{
					hit.SendMessage("OnMouseEnter");
					hit.SendMessage("OnMouseOver");
				}
			}
			m_LastHit[i] = hit;
		}
	}
}
