using System;
using System.Reflection.Emit;
using Xunit;

namespace EmitDebugging.Tests
{
	public static class CodeLineTests
	{
		[Fact]
		public static void CreateCodeLineViaCode()
		{
			var line = new CodeLine("code");
			Assert.Equal("code", line.Code);
			Assert.Equal(Indention.KeepCurrent, line.Indent);
			Assert.False(line.IsDebuggable);
			Assert.Equal(0, line.Labels.Length);
		}

		[Fact]
		public static void CreateCodeLineViaCodeAndIndention()
		{
			var line = new CodeLine("code", Indention.Increase);
			Assert.Equal("code", line.Code);
			Assert.Equal(Indention.Increase, line.Indent);
			Assert.False(line.IsDebuggable);
			Assert.Equal(0, line.Labels.Length);
		}

		[Fact]
		public static void CreateCodeLineViaCodeAndIsDebuggable()
		{
			var line = new CodeLine("code", true);
			Assert.Equal("code", line.Code);
			Assert.Equal(Indention.KeepCurrent, line.Indent);
			Assert.True(line.IsDebuggable);
			Assert.Equal(0, line.Labels.Length);
		}

		[Fact]
		public static void CreateCodeLineViaCodeAndIndentionAndIsDebuggable()
		{
			var line = new CodeLine("code", Indention.Decrease, true);
			Assert.Equal("code", line.Code);
			Assert.Equal(Indention.Decrease, line.Indent);
			Assert.True(line.IsDebuggable);
			Assert.Equal(0, line.Labels.Length);
		}

		[Fact]
		public static void CreateCodeLineViaCodeAndIndentionAndLabels()
		{
			var line = new CodeLine("code", Indention.Increase, new Label());
			Assert.Equal("code", line.Code);
			Assert.Equal(Indention.Increase, line.Indent);
			Assert.False(line.IsDebuggable);
			Assert.Equal(1, line.Labels.Length);
		}

		[Fact]
		public static void CreateCodeLineViaCodeAndSetCode()
		{
			var line = new CodeLine("code");
			line.Code = "new code";
			Assert.Equal("new code", line.Code);
		}

		[Fact]
		public static void CreateCodeLineViaCodeAndSetCodeNull()
		{
			var line = new CodeLine("code");
			Assert.Throws<ArgumentNullException>(() => line.Code = null);
		}

		[Fact]
		public static void CreateCodeLineViaCodeAsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new CodeLine(null));
		}
	}
}
