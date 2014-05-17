EmitDebugger
================

This is an assembly that generates a .il source file for emitted code with the correct breakpoints built-in for debugging purposes. It has the same binary interface with the Builder classes in System.Reflection.Emit to make it relatively painless to replace current code with this API.