using System;

namespace SIEDA.MonadicTests.HelperClass
{
   public class MyExceptionWithTypicalEquals : Exception
   {
      public string Text { get; private set; }

      public MyExceptionWithTypicalEquals()
      {
         Text = "Text";
      }

      public MyExceptionWithTypicalEquals( string text )
      {
         Text = text;
      }

      public override bool Equals( object obj )
      {
         if( obj is MyExceptionWithTypicalEquals p )
            return ( p.Text.Equals( Text ) );
         return false;
      }

      public override int GetHashCode() => Text.GetHashCode();
   }
}
