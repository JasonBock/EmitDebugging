using System;

namespace EmitDebugging
{
	public abstract class Debugging<T>
		: IDisposable
	{
		public abstract void Dispose();

		public T Builder { get; protected set; }
	}
}
