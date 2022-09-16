using System;
using SIEDA.MonadicTests.HelperClass;
using NUnit.Framework;
using SIEDA.Monadic;

namespace SIEDA.MonadicTests
{
   [TestFixture]
   [Description( "Prüft Verhalten von SIEDA's Option<T,E>." )]
   public class OptionTest
   {
      #region Construction

      [Test]
      public void ConstructNone_ViaSomeIsNull()
      {
         var testValue = Option<object, object>.From( null );

         Assert.That( testValue.IsSome, Is.False );
         Assert.That( testValue.IsNone, Is.True );
         Assert.That( testValue.IsFailure, Is.False );
      }

      [Test]
      public void ConstructNone_ViaNullableWithoutValue()
      {
         var testValue = Option<int, object>.From( new int?() );

         Assert.That( testValue.IsNone, Is.True );
         Assert.That( testValue.IsSome, Is.False );
         Assert.That( testValue.IsFailure, Is.False );
      }

      [Test]
      public void ExplicitNullThrowsException()
      {
         Assert.Throws<OptionSomeConstructionException>( () => Option<string, object>.Some( null ) );
      }

      [Test]
      public void ExplicitNullableThrowsException()
      {
         Assert.Throws<OptionSomeConstructionException>( () => Option<int, object>.Some( new int?() ) );
      }

      [Test]
      public void ConstructSome_ViaActualValue()
      {
         var testValue = Option<object, object>.Some( new object() );

         Assert.That( testValue.IsSome, Is.True );
         Assert.That( testValue.IsNone, Is.False );
         Assert.That( testValue.IsFailure, Is.False );
      }

      [Test]
      public void ConstructSome_ViaActualNullableValue()
      {
         int? testObject = 1;
         var testValue = Option<int, int>.Some( testObject );

         Assert.That( testValue.IsSome, Is.True );
         Assert.That( testValue.IsNone, Is.False );
         Assert.That( testValue.IsFailure, Is.False );
         Assert.That( testValue.Or( 2 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void ConstructFailure_ErrorNull()
      {
         Assert.Throws<OptionFailureConstructionException>( () => Option<object, object>.Failure( null ) );
      }

      [Test]
      public void ConstructFailure_ErrorNotNull()
      {
         var testValue = Option<object, object>.Failure( new object() );

         Assert.That( testValue.IsSome, Is.False );
         Assert.That( testValue.IsNone, Is.False );
         Assert.That( testValue.IsFailure, Is.True );
      }

      [Test]
      [Description( "Kann erfolgreiche Options vom Wertetyp 'Exception' konstruieren." )]
      public void ConstructSome_ValueIsAnException()
      {
         //WICHTIG: Es gibt Argumente, das hier zu verbieten und analog zum Option auf ein Failure zu mappen
         //         Es gibt aber auch Argumente, es so zu lassen wie es ist. Aktuell wurde sich für die
         //         weniger invasive Variante entschieden, vor allem da es weniger implizite Sachen macht.
         //         Dieser Test dient zur Dokumentation dieses Verhaltens.
         var x = Option<Exception, string>.Some( new Exception() );

         Assert.That( x.IsSome, Is.True );
         Assert.That( x.IsNone, Is.False );
         Assert.That( x.IsFailure, Is.False );
      }


      [Test]
      [Description( "Nesting-Ebenen werden beachtet." )]
      public void Nesting()
      {
         var someInnerSome = Option<Option<int, string>, string>.Some( Option<int, string>.Some( 123 ) );
         var someInnerNone = Option<Option<int, string>, string>.Some( Option<int, string>.None );
         var someInnerFail = Option<Option<int, string>, string>.Some( Option<int, string>.Failure( "hallo" ) );
         var failureInnerSome = Option<int, Option<int, string>>.Failure( Option<int, string>.Some( 123 ) );
         var failureInnerNone = Option<int, Option<int, string>>.Failure( Option<int, string>.None );
         var failureInnerFail = Option<int, Option<int, string>>.Failure( Option<int, string>.Failure( "hallo" ) );
         var none = Option<int, Option<int, string>>.None;

         Assert.That( someInnerSome.IsSome, Is.True );
         Assert.That( someInnerNone.IsSome, Is.True );
         Assert.That( someInnerFail.IsSome, Is.True );
         Assert.That( someInnerSome.OrThrow().IsSome, Is.True );
         Assert.That( someInnerNone.OrThrow().IsNone, Is.True );
         Assert.That( someInnerFail.OrThrow().IsFailure, Is.True );
         Assert.That( failureInnerSome.IsFailure, Is.True );
         Assert.That( failureInnerNone.IsFailure, Is.True );
         Assert.That( failureInnerFail.IsFailure, Is.True );
         Assert.That( failureInnerSome.FailureOrThrow().IsSome, Is.True );
         Assert.That( failureInnerNone.FailureOrThrow().IsNone, Is.True );
         Assert.That( failureInnerFail.FailureOrThrow().IsFailure, Is.True );
         Assert.That( none.IsNone, Is.True );
      }

      #endregion Construction

      #region ToString

      private class TestObj
      {
         private readonly string S;

         public TestObj( string s )
         {
            S = s;
         }

         public override string ToString() => $"Object '{S}'";
      }

      [Test]
      public void ToString_Some()
      {
         Assert.That( Option<TestObj, string>.From( new TestObj( "hallo" ) ).ToString(), Is.EqualTo( "[Option<TestObj, String>.Some: Object 'hallo']" ) );
      }

      [Test]
      public void ToString_None()
      {
         Assert.That( Option<TestObj, int>.From( null ).ToString(), Is.EqualTo( "[Option<TestObj, Int32>.None]" ) );
      }

      [Test]
      public void ToString_Failure()
      {
         Assert.That( Option<bool, TestObj>.Failure( new TestObj( "evil" ) ).ToString(), Is.EqualTo( "[Option<Boolean, TestObj>.Failure: Object 'evil']" ) );
      }

      #endregion ToString

      #region Equals

      [Test]
      [Description( "Beide haben denselben Wert." )]
      public void Equals_SomeEqual()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, int>( "abc", 123 );

         var x = Option<Tuple<string, int>, string>.Some( t1 );
         var y = Option<Tuple<string, int>, string>.Some( t2 );

         Assert.That( t1.Equals( t2 ), Is.True );
         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Beide haben Werte, aber unterschiedlich." )]
      public void Equals_SomeInequal()
      {
         var x = Option<int, string>.Some( 4 );
         var y = Option<int, string>.Some( 5 );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Beide haben denselben Fehler." )]
      public void Equals_FailedEqual()
      {
         var x = Option<object, int>.Failure( 123 );
         var y = Option<object, int>.Failure( 123 );

         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Es ist NICHT egal, welches Problem ein gescheitertes Option hat" )]
      public void Equals_FailedInequal()
      {
         var x = Option<object, string>.Failure( "what a disappointment" );
         var y = Option<object, string>.Failure( "yet another disappointment" );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Gescheitertes Option ist ungleich jedem Erfolgreichen." )]
      public void Equals_FailedInequalToSome()
      {
         var x = Option<int, string>.Failure( "how appaling!" );
         var y = Option<int, string>.Some( 0 );
         var z = Option<Option<int, string>, string>.Some( y );

         Assert.That( x.Equals( y ), Is.False );
         Assert.That( x.Equals( z ), Is.False );
      }

      [Test]
      [Description( "Leeres Option ist nur gleich andere leeren Options." )]
      public void Equals_None()
      {
         var some = Option<string, string>.Some( "something" );
         var fail = Option<string, string>.Failure( "inexcusable" );

         Assert.That( Option<string, string>.None.Equals( some ), Is.False );
         Assert.That( Option<string, string>.None.Equals( fail ), Is.False );
         Assert.That( Option<string, string>.None.Equals( Option<string, string>.None ), Is.True );
      }

      [Test]
      [Description( "Options zu unterschiedlichen Typen sind unterschiedlich." )]
      public void Equals_DifferentTypeInequal()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, string>( "abc", "123" );

         var x = Option<Tuple<string, int>, string>.Some( t1 );
         var y = Option<Tuple<string, string>, string>.Some( t2 );

         Assert.That( t1.Equals( t2 ), Is.False );
         Assert.That( x.Equals( y ), Is.False );
      }
      

      [Test]
      [Description( "Options zu unterschiedlichen Typen sind unterschiedlich - für Failure wird keine Ausnahme gemacht." )]
      public void Equals_DifferentTypeInequalForFailure()
      {
         var x = Option<int, string>.Failure( "horrible" );
         var y = Option<object, string>.Failure( "horrible" );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Options zu unterschiedlichen Typen sind unterschiedlich - für None wird keine Ausnahme gemacht." )]
      public void Equals_DifferentTypeInequalForNone()
      {
         var x = Option<int, string>.None;
         var y = Option<object, string>.None;

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Options zu unterschiedlichen Typen sind unterschiedlich, auch bei Subtypen im None-Fall!" )]
      public void DifferentInheritedType_None()
      {
         var x = Option<MyClassWithTypicalValueBasedEquals, MyExceptionWithTypicalEquals>.None;
         var y = Option<MySubclassWithTypicalValueBasedEquals, MySubexceptionWithTypicalValueBasedEquals>.None;

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Options zu unterschiedlichen Typen sind unterschiedlich, auch bei Subtypen im Failure-Fall!" )]
      public void DifferentInheritedType_Failure()
      {
         var sameText = "ANY_VALUE";
         var a = new MyExceptionWithTypicalEquals( sameText );
         var b = new MySubexceptionWithTypicalValueBasedEquals( sameText, 123 );
         Assert.That( a.Equals( b ), Is.True, "TEST-SETUP IS BROKEN!" );
         Assert.That( b.Equals( a ), Is.True, "TEST-SETUP IS BROKEN!" );

         var x = Option<MyClassWithTypicalValueBasedEquals, MyExceptionWithTypicalEquals>.Failure( a );
         var y = Option<MySubclassWithTypicalValueBasedEquals, MySubexceptionWithTypicalValueBasedEquals>.Failure( b );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Options zu unterschiedlichen Typen sind unterschiedlich, auch bei Subtypen im Some-Fall!" )]
      public void DifferentInheritedType_Success()
      {
         var sameText = "ANY_VALUE";
         var a = new MyClassWithTypicalValueBasedEquals( sameText );
         var b = new MySubclassWithTypicalValueBasedEquals( sameText, 123 );
         Assert.That( a.Equals( b ), Is.True, "TEST-SETUP IS BROKEN!" );
         Assert.That( b.Equals( a ), Is.True, "TEST-SETUP IS BROKEN!" );

         var x = Option<MyClassWithTypicalValueBasedEquals, MyExceptionWithTypicalEquals>.Some( a );
         var y = Option<MySubclassWithTypicalValueBasedEquals, MySubexceptionWithTypicalValueBasedEquals>.Some( b );

         Assert.That( x.Equals( y ), Is.False );
      }
      #endregion Equals

      #region Accessing Value
      [Test]
      public void Or_FailureOrNull()
      {
         Assert.That( Option<object, int>.Failure( -1 ).Or( null ), Is.Null );
      }

      [Test]
      public void Or_FailureOrValue()
      {
         Assert.That( Option<int, int>.Failure( -1 ).Or( 1 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void Or_Value()
      {
         Assert.That( Option<int, int>.Some( 1 ).Or( 2 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void Error_ThrowsIfSome()
      {
         Assert.Throws<OptionNoFailureException>( () => Option<string, int>.Some( "HAPPY" ).FailureOrThrow() );
      }

       [Test]
      public void Error_ReturnsFailure()
      {
         Assert.That( Option<string, int>.Failure( -1 ).FailureOrThrow(), Is.EqualTo( -1 ) );
      }

      [Test]
      public void OrThrow_ThrowsIfFailure()
      {
         Assert.Throws<OptionFailureException>( () => Option<string, int>.Failure( -1 ).OrThrow() );
      }
      

      [Test]
      public void OrThrow_DoesNotThrowIfSome()
      {
         var testValue = ""; //to be overwritten by "happy"

         Assert.DoesNotThrow( () => testValue = Option<string, int>.Some( "HAPPY" ).OrThrow() );
         Assert.That( testValue, Is.EqualTo( "HAPPY" ) );
      }

      [Test]
      public void OrThrowWithText_DoesThrowIfFailure()
      {
         Assert.Throws<OptionFailureException>( () => Option<string, int>.Failure( -1 ).OrThrow( "Test" ) );
      }

      [Test]
      public void OrThrowWithText_DoesNotThrowIfSome()
      {
         var testValue = 0;

         Assert.DoesNotThrow( () => testValue = Option<int, int>.Some( 1 ).OrThrow( "Test" ) );
         Assert.That( testValue, Is.EqualTo( 1 ) );
      }

      #endregion Accessing Value

      #region Subtypes

      [Test]
      public void OrWithDirectSubtype_Some()
      {
         var myClass = new MyClassWithTypeBasedEquals();
         var mySubclass = new MySubclassWithTypeBasedEquals();

         var underTest = Option<MyClassWithTypeBasedEquals, Exception>.Some( myClass );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( myClass ) );
      }

      [Test]
      public void OrWithDirectSubtype_None()
      {
         var mySubclass = new MySubclassWithTypeBasedEquals();

         var underTest = Option<MyClassWithTypeBasedEquals, Exception>.None;
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithDirectSubtype_Failure()
      {
         var mySubclass = new MySubclassWithTypeBasedEquals();

         var underTest = Option<MyClassWithTypeBasedEquals, Exception>.Failure( new ArgumentException( "irrelevant" ) );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelSubtype_Some()
      {
         var mySubclass = new MySubclassWithTypeBasedEquals();
         var myOtherSubclass = new MyOtherSubclassWithTypeBasedEquals();

         var underTest = Option<MyClassWithTypeBasedEquals, Exception>.Some( mySubclass );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelSubtype_None()
      {
         var myOtherSubclass = new MyOtherSubclassWithTypeBasedEquals();

         var underTest = Option<MyClassWithTypeBasedEquals, Exception>.None;
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( myOtherSubclass ) );
      }

      [Test]
      public void OrWithParallelSubtype_Failure()
      {
         var myOtherSubclass = new MyOtherSubclassWithTypeBasedEquals();

         var underTest = Option<MyClassWithTypeBasedEquals, Exception>.Failure( new ArgumentException( "irrelevant" ) );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( myOtherSubclass ) );
      }

      #endregion Subtypes

      #region Flatten
      [Test]
      public void Flatten_Some()
      {
         var a = Option<string, int>.Some( "hallo" );
         var b = Option<Option<string, int>, int>.Some( Option<string, int>.Some( "hallo" ) );
         var c = Option<Option<string, int>, int>.Some( Option<string, int>.None );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.Not.EqualTo( c ) );
      }

      [Test]
      public void Flatten_None()
      {
         var a = Option<string, int>.None;
         var b = Option<Option<string, int>, int>.Some( Option<string, int>.None );
         var c = Option<Option<string, int>, int>.Some( Option<string, int>.Failure( 42 ) );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.Not.EqualTo( c ) );
      }

      [Test]
      public void Flatten_Failure()
      {
         var a = Option<string, int>.Failure( 42 );
         var b = Option<Option<string, int>, int>.Some( Option<string, int>.Failure( 42 ) );
         var c = Option<Option<string, int>, int>.Some( Option<string, int>.None );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.Not.EqualTo( c ) );
      }

      [Test]
      public void Flatten_DifferentFailType_Some()
      {
         var a = Option<string, int>.Some( "hallo" );
         var b = Option<Option<string, string>, int>.Some( Option<string, string>.Some( "hallo" ) );
         var c = Option<Option<string, string>, int>.Some( Option<string, string>.None );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.Not.EqualTo( c ) );
      }

      [Test]
      public void Flatten_DifferentFailType_None()
      {
         var a = Option<string, int>.None;
         var b = Option<Option<string, string>, int>.Some( Option<string, string>.None );
         var c = Option<Option<string, string>, int>.Some( Option<string, string>.Failure( "error" ) );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.Not.EqualTo( c ) );
      }

      [Test]
      public void Flatten_DifferentFailType_Failure()
      {
         var a = Option<string, int>.Failure( 42 );
         var b = Option<Option<string, string>, int>.Some( Option<string, string>.Failure( "error" ) );
         var c = Option<Option<string, string>, int>.Some( Option<string, string>.None );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.Not.EqualTo( c ) );
      }
      #endregion Flatten

      #region Map and Nesting
      [Test]
      [Description( "Die Map-Operation wird auf Erfolge angewendet." )]
      public void Map_Some()
      {
         var original = Option<string, bool>.Some( "hallo" );
         var result = original.Map( s => s+= " welt" );
         Assert.That( result.OrThrow, Is.EqualTo( "hallo welt" ) );
      }

      [Test]
      [Description( "Die Map-Operation wird nicht auf Leerwerte angewendet." )]
      public void Map_None()
      {
         var original = Option<string, bool>.None;
         var result = original.Map( s => s + "!!!" );
         Assert.That( result.IsNone, Is.True );
      }

      [Test]
      [Description( "Die Map-Operation wird nicht auf Fehlschläge angewendet." )]
      public void Map_Failure()
      {
         var original = Option<string, bool>.Failure( false );
         var result = original.Map( s => s+= " welt" );
         Assert.That( result.FailureOrThrow, Is.EqualTo( false ));
      }

      [Test]
      [Description( "Die Map-Operation ist kompatibel mit Verschachtelung" )]
      public void Map_NestingInMap()
      {
         var flag1 = Option<bool, string>.Some( true );

         var result = flag1.Map( _ => Option<bool, string>.Some( true ) );

         Assert.That( result.Or( Option<bool, string>.Failure( "disgusting" ) ).Or( false ), Is.True );
      }

      [Test]
      [Description( "Map hat keine Probleme mit Typveränderung, weder zur Lauf- noch zur Compilezeit." )]
      public void MapToDifferentType()
      {
         var one = Option<int, bool>.Some( 1 );
         Option<string, bool> onePlusOne = one.Map( i => $"{i}+1=2" );

         Assert.That( onePlusOne.OrThrow(), Is.EqualTo( "1+1=2" ) );
      }

      [Test]
      [Description( "FlatMap hat keine Probleme mit Typveränderung, weder zur Lauf- noch zur Compilezeit." )]
      public void FlatMapToDifferentType()
      {
         var one = Option<int, bool>.Some( 1 );
         Option<string, bool> onePlusOne = one.FlatMap( i => Option<string, bool>.Some( $"{i}+1=2" ) );

         Assert.That( onePlusOne.OrThrow(), Is.EqualTo( "1+1=2" ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit FlatMap." )]
      public void Map_NestingInFlatMap()
      {
         var flag1 = Option<int, string>.Some( 1 );
         var flag2 = Option<int, string>.Some( 2 );

         var result = flag1.FlatMap( outerInt => flag2.Map( innerInt => outerInt + innerInt  ) );

         Assert.That( result.Or( -999 ), Is.EqualTo( 3 ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit Wertübergabe mit FlatMap." )]
      public void ValuePropagationInFlatMap()
      {
         var hallo = Option<string, int>.Some( "hallo" );
         var sp = Option<string, int>.Some( " " );

         var result = hallo.FlatMap( h => sp.Map( space => h + space + "welt" ) );

         Assert.That( result.Or( "nix da" ), Is.EqualTo( "hallo welt" ) );
      }
      #endregion Map and Nesting

      #region Is

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'true'" )]
      public void Is_DefinedTrue()
      {
         var f = Option<string, Exception>.Some( "hubba" );

         Assert.That( f.Is( "hubba" ), Is.EqualTo( true ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'false'" )]
      public void Is_DefinedFalse()
      {
         var f = Option<string, Exception>.Some( "hubba" );

         Assert.That( f.Is( "whatever" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - None immer 'false'" )]
      public void Is_FalseWhenNone()
      {
         var f = Option<string, Exception>.None;

         Assert.That( f.Is( "hubba" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - None immer 'false'" )]
      public void Is_FalseWhenFailed()
      {
         var f = Option<string, Exception>.Failure( new ArgumentException() );

         Assert.That( f.Is( "hubba" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'true'" )]
      public void IsNot_DefinedTrue()
      {
         var f = Option<string, Exception>.Some( "hubba" );

         Assert.That( f.IsNot( "hubbahub" ), Is.EqualTo( true ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'false'" )]
      public void IsNot_DefinedFalse()
      {
         var f = Option<string, Exception>.Some( "hubba" );

         Assert.That( f.IsNot( "hubba" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - None immer 'false'" )]
      public void IsNot_FalseWhenNone()
      {
         var f = Option<string, Exception>.None;

         Assert.That( f.IsNot( "hubba" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - None immer 'false'" )]
      public void IsNot_FalseWhenFailed()
      {
         var f = Option<string, Exception>.Failure( new ArgumentException() );

         Assert.That( f.IsNot( "hubba" ), Is.EqualTo( false ) );
      }

      #endregion Is

      #region Holds

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'true'" )]
      public void Is_PredicateMatchingDefined()
      {
         var option = Option<string, Exception>.Some( "hubba" );

         Assert.That( option.Holds( s => s.Equals( "hubba" ) ), Is.EqualTo( true ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'false'" )]
      public void Is_PredicateNonMatchingDefined()
      {
         var option = Option<string, Exception>.Some( "hubba" );

         Assert.That( option.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich findet nicht statt" )]
      public void Is_PredicateUndefined()
      {
         var option = Option<string, Exception>.Failure( new ArgumentException() );

         Assert.That( option.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      #endregion Holds

      #region TryGet

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei Failure" )]
      public void TryGet_Result_Failure()
      {
         var option = Option<string, Exception>.Failure( new ArgumentException() );

         Assert.IsFalse( option.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei Failure" )]
      public void TryGet_Value_Failure()
      {
         var option = Option<string, Exception>.Failure( new ArgumentException() );
         option.TryGetValue( out var s );

         Assert.IsNull( s );
      }

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei Some" )]
      public void TryGet_Value_Some()
      {
         var option = Option<string, Exception>.Some( "blub" );

         Assert.IsTrue( option.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei Some" )]
      public void TryGet_Result_Some()
      {
         var option = Option<string, Exception>.Some( "blub" );
         option.TryGetValue( out var s );

         Assert.That( s, Is.EqualTo( "blub" ) );
      }

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei None" )]
      public void TryGet_Value_None()
      {
         var option = Option<string, Exception>.None;

         Assert.IsFalse( option.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei None" )]
      public void TryGet_Result_None()
      {
         var option = Option<string, Exception>.None;
         option.TryGetValue( out var s );

         Assert.IsNull( s );
      }

      #endregion TryGet

      #region TryGetFailure

      [Test]
      [Description( "TryGetFailure() produziert korrektes Boolean-Result bei Some" )]
      public void TryGetFailure_Some()
      {
         var option = Option<string, Exception>.Some( "blah" );

         Assert.IsFalse( option.TryGetFailure( out var s ) );
      }

      [Test]
      [Description( "TryGetFailure() schreibt keinen Wert bei Some" )]
      public void TryGetFailure_Result_Some()
      {
         var option = Option<string, Exception>.Some( "blah" );
         option.TryGetFailure( out var s );

         Assert.IsNull( s );
      }

      [Test]
      [Description( "TryGetFailure() produziert korrektes Boolean-Result bei Failure" )]
      public void TryGetFailure_Failure()
      {
         var option = Option<string, Exception>.Failure( new ArgumentException() );

         Assert.IsTrue( option.TryGetFailure( out var s ) );
      }

      [Test]
      [Description( "TryGetFailure() schreibt Wert bei Failure" )]
      public void TryGetFailure_Result_Failure()
      {
         var option = Option<string, Exception>.Failure( new ArgumentException("msg") );
         option.TryGetFailure( out var e );

         Assert.That( e, Is.TypeOf<ArgumentException>() );
         Assert.That( e.Message, Is.EqualTo("msg") );
      }

      [Test]
      [Description( "TryGetFailure() produziert korrektes Boolean-Result bei None" )]
      public void TryGetFailure_None()
      {
         var option = Option<string, Exception>.None;

         Assert.IsFalse( option.TryGetFailure( out var s ) );
      }

      [Test]
      [Description( "TryGetFailure() schreibt keinen Wert bei None" )]
      public void TryGetFailure_Result_None()
      {
         var option = Option<string, Exception>.None;
         option.TryGetFailure( out var s );

         Assert.IsNull( s );
      }

      #endregion TryGetFailure

      #region Convert
      
      [Test]
      [Description( "Some wird zu Maybe.Some konvertiert" )]
      public void ConvertToMaybe_Some()
      {
         var option = Option<string, int>.Some( "hallo" );
         var maybe = option.ToMaybe();

         Assert.That( maybe.IsSome, Is.True );
         Assert.That( maybe.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Failure wird zu Maybe.None konvertiert" )]
      public void ConvertToMaybe_Failure()
      {
         var option = Option<string, int>.Failure( 666 );
         var maybe = option.ToMaybe();

         Assert.That( maybe.IsNone, Is.True );
      }

      [Test]
      [Description( "None wird zu Maybe.None konvertiert" )]
      public void ConvertToMaybe_None()
      {
         var option = Option<int, string>.None;
         var maybe = option.ToMaybe();

         Assert.That( maybe.IsNone, Is.True );
      }

      [Test]
      [Description( "Some wird zu Failable.Some konvertiert" )]
      public void ConvertToFailable_Some()
      {
         var option = Option<int, string>.Some( 123 );
         var failable = option.ToFailable( "whatever" );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( 123 ) );
      }

      [Test]
      [Description( "Failure wird zu Failable.Failure konvertiert" )]
      public void ConvertToFailable_Failure()
      {
         var option = Option<int, string>.Failure( "msg" );
         var failable = option.ToFailable( "notMsg" ); //different text!

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow(), Is.EqualTo("msg") );
      }

      [Test]
      [Description( "None wird zu Failable.Failure konvertiert" )]
      public void ConvertToFailable_None()
      {
         var option = Option<int, string>.None;
         var failable = option.ToFailable( "msg" );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow(), Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "Some wird zu EFailable.Success konvertiert" )]
      public void ConvertToEFailable_Some()
      {
         var option = Option<int, string>.Some( 123 );
         var failable = option.ToEFailable( new ArgumentException() );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( 123 ) );
      }

      [Test]
      [Description( "Failure wird zu EFailable.Failure konvertiert" )]
      public void ConvertToEFailable_Failure()
      {
         var option = Option<int, string>.Failure( "notMsg" );
         EFailable<int> failable = option.ToEFailable( new ArgumentException( "msg" ) );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "None wird zu EFailable.Failure konvertiert" )]
      public void ConvertToEFailable_None()
      {
         var option = Option<int, string>.None;
         var failable = option.ToEFailable( new ArgumentException( "msg" ) );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "Some wird zu Failable.Some konvertiert" )]
      public void OrConvertToFailable_Some()
      {
         var option = Option<int, string>.Some( 123 );
         var failable = option.OrToFailable( 456 );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( 123 ) );
      }

      [Test]
      [Description( "Failure wird zu Failable.Failure konvertiert" )]
      public void OrConvertToFailable_Failure()
      {
         var option = Option<int, string>.Failure( "msg" );
         var failable = option.OrToFailable( 456 );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow(), Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "None wird zu Failable.Success mit übergebenem Wert konvertiert" )]
      public void OrConvertToFailable_None()
      {
         var option = Option<int, string>.None;
         var failable = option.OrToFailable( 456 );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( 456 ) );
      }

      [Test]
      [Description( "Some wird zu EFailable.Success konvertiert" )]
      public void OrConvertToEFailable_Some()
      {
         var option = Option<int, string>.Some( 123 );
         var failable = option.OrToEFailable( 456, new ArgumentException( "msg" ) );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( 123 ) );
      }

      [Test]
      [Description( "Failure wird zu EFailable.Failure konvertiert" )]
      public void OrConvertToEFailable_Failure()
      {
         var option = Option<int, string>.Failure( "notMsg" );
         EFailable<int> failable = option.OrToEFailable( 456, new ArgumentException( "msg" ) );

         Assert.That( failable.IsFailure, Is.True );
         Assert.That( failable.FailureOrThrow().Message, Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "None wird zu EFailable.Success mit übergebenem Wert konvertiert" )]
      public void OrConvertToEFailable_None()
      {
         var option = Option<int, string>.None;
         var failable = option.OrToEFailable( 456, new ArgumentException( "msg" ) );

         Assert.That( failable.IsSuccess, Is.True );
         Assert.That( failable.OrThrow, Is.EqualTo( 456 ) );
      }

      [Test]
      [Description( "Some wird zu EOption.Some konvertiert" )]
      public void ConvertToEFailable_WithConverter_Some()
      {
         var testValue = Option<int, string>.Some( 42 );
         var eOption = testValue.ToEOptionWith( i => $"1{i}", new ArgumentException() );

         Assert.That( eOption.IsSome, Is.True );
         Assert.That( eOption.OrThrow, Is.EqualTo( "142" ) );
      }

      [Test]
      [Description( "Failure wird zu EOption.Failure konvertiert" )]
      public void ConvertToEFailable_WithConverter_Failure()
      {
         var testValue = Option<int, string>.Failure( "will-be-overwritten" );
         var eOption = testValue.ToEOptionWith( i => $"1{i}", new ArgumentException("msg") );

         Assert.That( eOption.IsFailure, Is.True );
         Assert.That( eOption.FailureOrThrow().Message, Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "None wird zu EOption.None konvertiert" )]
      public void ConvertToEFailable_WithConverter_None()
      {
         var testValue = Option<int, ArgumentException>.None;
         var eOption = testValue.ToEOptionWith( i => $"1{i}", new ArgumentException() );

         Assert.That( eOption.IsNone, Is.True );
      }
      #endregion Convert
   }
}