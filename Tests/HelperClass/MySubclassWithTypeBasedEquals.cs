namespace SIEDA.MonadicTests.HelperClass
{
   public class MySubclassWithTypeBasedEquals : MyClassWithTypeBasedEquals
   {
      public int Number { get; private set; }

      public MySubclassWithTypeBasedEquals() : this( "subtext", 42 ) { }

      public MySubclassWithTypeBasedEquals( string text, int number ) : base( text )
      {
         Number = number;
      }
   }
}
