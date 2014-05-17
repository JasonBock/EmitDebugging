using System.Diagnostics.CodeAnalysis;

namespace EmitDebugging.Tests.Types
{
	public static class VarArgMethod
	{
		[SuppressMessage("Microsoft.Usage", "CA2230:UseParamsForVariableArguments")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public static void VarArg(int value, __arglist) { }

		public static void CallVarArg()
		{
			VarArgMethod.VarArg(22, __arglist(3));
		}
	}
}
