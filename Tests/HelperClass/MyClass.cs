namespace MonadicTests.Tests.HelperClass
{
   public class MyClass
   {
      public string Text { get; private set; }

      public MyClass()
      {
         Text = "Text";
      }

      public MyClass( string text )
      {
         Text = text;
      }

      public override bool Equals( object obj )
      {
         if( ( obj == null ) || !GetType().Equals( obj.GetType() ) )
         {
            return false;
         }
         else
         {
            var p = (MyClass) obj;
            return ( p.Text.Equals( Text ) );
         }
      }

      public override int GetHashCode() => Text.GetHashCode();
   }
}
