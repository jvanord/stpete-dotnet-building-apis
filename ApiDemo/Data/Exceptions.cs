using System;

namespace ApiDemo.Data.Exceptions
{
	public class ResourceException : Exception
	{
		public ResourceException() : base() { }
		public ResourceException(string message) : base(message) { }
	}
	public class ResourceNotFoundException : ResourceException
	{
		public ResourceNotFoundException() : base() { }
		public ResourceNotFoundException(string message) : base(message) { }
	}
}