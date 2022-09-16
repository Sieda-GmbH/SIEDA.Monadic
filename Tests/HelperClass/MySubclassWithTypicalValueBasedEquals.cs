namespace SIEDA.MonadicTests.HelperClass
{
   public class MySubclassWithTypicalValueBasedEquals : MyClassWithTypicalValueBasedEquals
   {
      public int Number { get; private set; }

      public MySubclassWithTypicalValueBasedEquals() : this( "subtext", 42 ) { }

      public MySubclassWithTypicalValueBasedEquals( string text, int number ) : base( text )
      {
         Number = number;
      }
   }
}
