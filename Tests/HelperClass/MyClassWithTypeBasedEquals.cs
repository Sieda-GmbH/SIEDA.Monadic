namespace SIEDA.MonadicTests.HelperClass
{
   public class MyClassWithTypeBasedEquals
   {
      public string Text { get; private set; }

      public MyClassWithTypeBasedEquals()
      {
         Text = "Text";
      }

      public MyClassWithTypeBasedEquals( string text )
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
            var p = (MyClassWithTypeBasedEquals) obj;
            return ( p.Text.Equals( Text ) );
         }
      }

      public override int GetHashCode() => Text.GetHashCode();
   }
}
