namespace Accessories.Exceptions;

[Serializable]
public class AuthorizationException : Exception
{
	public AuthorizationException() { }
	public AuthorizationException(string message) : base(message) { }
	public AuthorizationException(string message, Exception inner) : base(message, inner) { }
	protected AuthorizationException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
