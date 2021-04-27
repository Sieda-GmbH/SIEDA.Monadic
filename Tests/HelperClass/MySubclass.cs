namespace SIEDA.MonadicTests.HelperClass
{
   public class MySubclass : MyClass
   {
      public int Number { get; private set; }

      public MySubclass() : this( "subtext", 42 ) { }

      public MySubclass( string text, int number ) : base( text )
      {
         Number = number;
      }
   }
}
