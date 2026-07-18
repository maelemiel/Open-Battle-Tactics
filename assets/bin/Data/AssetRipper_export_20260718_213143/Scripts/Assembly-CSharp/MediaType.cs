public interface MediaType
{
	object Marshall(object o);

	T Unmarshall<T>(RestResponse response);

	object ParseResponseStream(RestResponse response);
}
