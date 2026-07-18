using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class ParticleSystem : Component
	{
		public struct Particle
		{
			private Vector3 m_Position;

			private Vector3 m_Velocity;

			private Vector3 m_AnimatedVelocity;

			private Vector3 m_AxisOfRotation;

			private float m_Rotation;

			private float m_AngularVelocity;

			private float m_Size;

			private Color32 m_Color;

			private uint m_RandomSeed;

			private float m_Lifetime;

			private float m_StartLifetime;

			private float m_EmitAccumulator0;

			private float m_EmitAccumulator1;

			public Vector3 position
			{
				get
				{
					return m_Position;
				}
				set
				{
					m_Position = value;
				}
			}

			public Vector3 velocity
			{
				get
				{
					return m_Velocity;
				}
				set
				{
					m_Velocity = value;
				}
			}

			public float lifetime
			{
				get
				{
					return m_Lifetime;
				}
				set
				{
					m_Lifetime = value;
				}
			}

			public float startLifetime
			{
				get
				{
					return m_StartLifetime;
				}
				set
				{
					m_StartLifetime = value;
				}
			}

			public float size
			{
				get
				{
					return m_Size;
				}
				set
				{
					m_Size = value;
				}
			}

			public Vector3 axisOfRotation
			{
				get
				{
					return m_AxisOfRotation;
				}
				set
				{
					m_AxisOfRotation = value;
				}
			}

			public float rotation
			{
				get
				{
					return m_Rotation * 57.29578f;
				}
				set
				{
					m_Rotation = value * ((float)Math.PI / 180f);
				}
			}

			public float angularVelocity
			{
				get
				{
					return m_AngularVelocity * 57.29578f;
				}
				set
				{
					m_AngularVelocity = value * ((float)Math.PI / 180f);
				}
			}

			public Color32 color
			{
				get
				{
					return m_Color;
				}
				set
				{
					m_Color = value;
				}
			}

			[Obsolete("randomValue property is deprecated. Use randomSeed instead to control random behavior of particles.")]
			public float randomValue
			{
				get
				{
					return BitConverter.ToSingle(BitConverter.GetBytes(m_RandomSeed), 0);
				}
				set
				{
					m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
				}
			}

			public uint randomSeed
			{
				get
				{
					return m_RandomSeed;
				}
				set
				{
					m_RandomSeed = value;
				}
			}
		}

		public struct CollisionEvent
		{
			private Vector3 m_Intersection;

			private Vector3 m_Normal;

			private Vector3 m_Velocity;

			private int m_ColliderInstanceID;

			public Vector3 intersection
			{
				get
				{
					return m_Intersection;
				}
			}

			public Vector3 normal
			{
				get
				{
					return m_Normal;
				}
			}

			public Vector3 velocity
			{
				get
				{
					return m_Velocity;
				}
			}

			public Collider collider
			{
				get
				{
					return InstanceIDToCollider(m_ColliderInstanceID);
				}
			}
		}

		public extern float startDelay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool isPlaying
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool isStopped
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool isPaused
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool loop
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool playOnAwake
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float duration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float playbackSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int particleCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int safeCollisionEventSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool enableEmission
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float emissionRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float startSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float startSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Color startColor
		{
			get
			{
				Color value;
				INTERNAL_get_startColor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_startColor(ref value);
			}
		}

		public extern float startRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float startLifetime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float gravityModifier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int maxParticles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern ParticleSystemSimulationSpace simulationSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern uint randomSeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern Collider InstanceIDToCollider(int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_startColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_startColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetParticles(Particle[] particles, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int GetParticles(Particle[] particles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int GetCollisionEvents(GameObject go, CollisionEvent[] collisionEvents);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_Simulate(float t, bool restart);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_Play();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_Stop();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_Pause();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_Clear();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool Internal_IsAlive();

		[ExcludeFromDocs]
		public void Simulate(float t, bool withChildren)
		{
			bool restart = true;
			Simulate(t, withChildren, restart);
		}

		[ExcludeFromDocs]
		public void Simulate(float t)
		{
			bool restart = true;
			bool withChildren = true;
			Simulate(t, withChildren, restart);
		}

		public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart)
		{
			if (withChildren)
			{
				ParticleSystem[] particleSystems = GetParticleSystems(this);
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Internal_Simulate(t, restart);
				}
			}
			else
			{
				Internal_Simulate(t, restart);
			}
		}

		[ExcludeFromDocs]
		public void Play()
		{
			bool withChildren = true;
			Play(withChildren);
		}

		public void Play([DefaultValue("true")] bool withChildren)
		{
			if (withChildren)
			{
				ParticleSystem[] particleSystems = GetParticleSystems(this);
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Internal_Play();
				}
			}
			else
			{
				Internal_Play();
			}
		}

		[ExcludeFromDocs]
		public void Stop()
		{
			bool withChildren = true;
			Stop(withChildren);
		}

		public void Stop([DefaultValue("true")] bool withChildren)
		{
			if (withChildren)
			{
				ParticleSystem[] particleSystems = GetParticleSystems(this);
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Internal_Stop();
				}
			}
			else
			{
				Internal_Stop();
			}
		}

		[ExcludeFromDocs]
		public void Pause()
		{
			bool withChildren = true;
			Pause(withChildren);
		}

		public void Pause([DefaultValue("true")] bool withChildren)
		{
			if (withChildren)
			{
				ParticleSystem[] particleSystems = GetParticleSystems(this);
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Internal_Pause();
				}
			}
			else
			{
				Internal_Pause();
			}
		}

		[ExcludeFromDocs]
		public void Clear()
		{
			bool withChildren = true;
			Clear(withChildren);
		}

		public void Clear([DefaultValue("true")] bool withChildren)
		{
			if (withChildren)
			{
				ParticleSystem[] particleSystems = GetParticleSystems(this);
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					particleSystem.Internal_Clear();
				}
			}
			else
			{
				Internal_Clear();
			}
		}

		[ExcludeFromDocs]
		public bool IsAlive()
		{
			bool withChildren = true;
			return IsAlive(withChildren);
		}

		public bool IsAlive([DefaultValue("true")] bool withChildren)
		{
			if (withChildren)
			{
				ParticleSystem[] particleSystems = GetParticleSystems(this);
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem particleSystem in array)
				{
					if (particleSystem.Internal_IsAlive())
					{
						return true;
					}
				}
				return false;
			}
			return Internal_IsAlive();
		}

		public void Emit(int count)
		{
			INTERNAL_CALL_Emit(this, count);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Emit(ParticleSystem self, int count);

		public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
		{
			Particle particle = new Particle
			{
				position = position,
				velocity = velocity,
				lifetime = lifetime,
				startLifetime = lifetime,
				size = size,
				rotation = 0f,
				angularVelocity = 0f,
				color = color,
				randomSeed = 5u
			};
			Internal_Emit(ref particle);
		}

		public void Emit(Particle particle)
		{
			Internal_Emit(ref particle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_Emit(ref Particle particle);

		internal static ParticleSystem[] GetParticleSystems(ParticleSystem root)
		{
			if (!root)
			{
				return null;
			}
			List<ParticleSystem> list = new List<ParticleSystem>();
			list.Add(root);
			GetDirectParticleSystemChildrenRecursive(root.transform, list);
			return list.ToArray();
		}

		private static void GetDirectParticleSystemChildrenRecursive(Transform transform, List<ParticleSystem> particleSystems)
		{
			foreach (Transform item in transform)
			{
				ParticleSystem component = item.gameObject.GetComponent<ParticleSystem>();
				if (component != null)
				{
					particleSystems.Add(component);
					GetDirectParticleSystemChildrenRecursive(item, particleSystems);
				}
			}
		}
	}
}
