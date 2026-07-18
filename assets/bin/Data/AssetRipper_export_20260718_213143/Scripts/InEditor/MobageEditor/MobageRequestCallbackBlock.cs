using System.Net;

namespace MobageEditor
{
	public delegate void MobageRequestCallbackBlock(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status);
}
