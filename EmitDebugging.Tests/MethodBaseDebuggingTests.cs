using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Xunit;

namespace EmitDebugging.Tests
{
	public sealed class MethodBaseDebuggingTests
		: AssemblyCreationTests
	{
		[Fact]
		public static void CallDisposeAfterBeginCatchBlock()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().BeginCatchBlock(null));
		}

		[Fact]
		public static void CallDisposeAfterBeginExceptFilterBlock()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().BeginExceptFilterBlock());
		}

		[Fact]
		public static void CallDisposeAfterBeginExceptionBlock()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().BeginExceptionBlock());
		}

		[Fact]
		public static void CallDisposeAfterBeginFaultBlock()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().BeginFaultBlock());
		}

		[Fact]
		public static void CallDisposeAfterBeginFinallyBlock()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().BeginFinallyBlock());
		}

		[Fact]
		public static void CallDisposeAfterBeginScope()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().BeginScope());
		}

		[Fact]
		public static void CallDisposeAfterDeclareLocalType()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().DeclareLocal(typeof(int)));
		}

		[Fact]
		public static void CallDisposeAfterDeclareLocalTypeBool()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().DeclareLocal(typeof(int), true));
		}

		[Fact]
		public static void CallDisposeAfterDefineLabel()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().DefineLabel());
		}

		[Fact]
		public static void CallDisposeAfterDispose()
		{
			MethodBaseDebuggingTests.GetMethod().Dispose();
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCode()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeByte()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, (byte)33));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeConstructorInfo()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, null as ConstructorInfo));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeDouble()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, 33d));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeFieldInfo()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, null as FieldInfo));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeFloat()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, 33f));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeInt()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, 33));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeLabel()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, new Label()));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeLabelArray()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, new Label[3]));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeLocalBuilder()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, null as LocalBuilder));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeLong()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, 33L));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeMethodInfo()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, null as MethodInfo));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeShort()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, (short)33));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeSignatureHelper()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, null as SignatureHelper));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeString()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, null as string));
		}

		[Fact]
		public static void CallDisposeAfterEmitOpCodeType()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().Emit(OpCodes.Ret, null as Type));
		}

		[Fact]
		public static void CallDisposeAfterEmitCall()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().EmitCall(
				OpCodes.Calli, null as MethodInfo, null as Type[]));
		}

		[Fact]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Calli")]
		public static void CallDisposeAfterEmitCalliManaged()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().EmitCalli(
				OpCodes.Calli, CallingConventions.Standard, null as Type, null as Type[], null as Type[]));
		}

		[Fact]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Calli")]
		public static void CallDisposeAfterEmitCalliUnmanaged()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().EmitCalli(
				OpCodes.Calli, CallingConvention.StdCall, null as Type, null as Type[]));
		}

		[Fact]
		public static void CallDisposeAfterEmitWriteLineFieldInfo()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().EmitWriteLine(null as FieldInfo));
		}

		[Fact]
		public static void CallDisposeAfterEmitWriteLineLocalBuilder()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().EmitWriteLine(null as LocalBuilder));
		}

		[Fact]
		public static void CallDisposeAfterEmitWriteLineString()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().EmitWriteLine(null as string));
		}

		[Fact]
		public static void CallDisposeAfterEndExceptionBlock()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().EndExceptionBlock());
		}

		[Fact]
		public static void CallDisposeAfterEndScope()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().EndScope());
		}

		[Fact]
		public static void CallDisposeAfterMarkLabel()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().MarkLabel(new Label()));
		}

		[Fact]
		public static void CallDisposeAfterThrowException()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().ThrowException(null));
		}

		[Fact]
		public static void CallDisposeAfterUsingNamespace()
		{
			Assert.Throws<ObjectDisposedException>(() => MethodBaseDebuggingTests.GetMethod().UsingNamespace(null));
		}

		private static MethodDebugging GetMethod()
		{
			MethodDebugging method = null;

			var name = Guid.NewGuid().ToString("N");

			using (var assembly = AssemblyCreationTests.CreateDebuggingAssembly(name))
			{
				using (var type = AssemblyCreationTests.CreateDebuggingType(
					assembly, assembly.Builder.GetDynamicModule(name), "Type"))
				{
					using (method = type.GetMethodDebugging(
						type.Builder.DefineMethod("Method", MethodAttributes.Public | MethodAttributes.Static))) { }
				}
			}

			return method;
		}
	}
}
