namespace SIEDA.MonadicTests.HelperClass
{
   public class MyClassWithTypicalValueBasedEquals
   {
      public string Text { get; private set; }

      public MyClassWithTypicalValueBasedEquals()
      {
         Text = "Text";
      }

      public MyClassWithTypicalValueBasedEquals( string text )
      {
         Text = text;
      }

      public override bool Equals( object obj )
      {
         if ( obj is MyClassWithTypicalValueBasedEquals p )
            return ( p.Text.Equals( Text ) );
         return false;
      }

      public override int GetHashCode() => Text.GetHashCode();
   }
}
