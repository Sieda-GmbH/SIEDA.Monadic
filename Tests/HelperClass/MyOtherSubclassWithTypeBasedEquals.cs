namespace SIEDA.MonadicTests.HelperClass
{
   public class MyOtherSubclassWithTypeBasedEquals : MyClassWithTypeBasedEquals
   {
      public float Fraction { get; private set; }

      public MyOtherSubclassWithTypeBasedEquals() : this( "othersubtext", 0.42f ) { }

      public MyOtherSubclassWithTypeBasedEquals( string text, float fraction ) : base( text )
      {
         Fraction = fraction;
      }
   }
}
