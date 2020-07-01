namespace MonadicTests.Tests.HelperClass
{
   public class MyOtherSubclass : MyClass
   {
      public float Fraction { get; private set; }

      public MyOtherSubclass() : this( "othersubtext", 0.42f ) { }

      public MyOtherSubclass( string text, float fraction ) : base( text )
      {
         Fraction = fraction;
      }
   }
}
