namespace SIEDA.MonadicTests.HelperClass
{
   public class MySubexceptionWithTypicalValueBasedEquals : MyExceptionWithTypicalEquals
   {
      public int Number { get; private set; }

      public MySubexceptionWithTypicalValueBasedEquals() : this( "subtext", 42 ) { }

      public MySubexceptionWithTypicalValueBasedEquals( string text, int number ) : base( text )
      {
         Number = number;
      }
   }
}
