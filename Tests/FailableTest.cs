﻿using System;
using SIEDA.MonadicTests.HelperClass;
using NUnit.Framework;
using SIEDA.Monadic;
using SIEDA.Monadic.SwitchCase;

namespace SIEDA.MonadicTests
{
   [TestFixture]
   [Description( "Prüft Verhalten von SIEDA's Failable<T,E>." )]
   public class FailableTest
   {
      #region Construction

      [Test]
      public void ConstructSuccess_Null()
      {
         Assert.That( () => Failable<object, object>.Success( null ), Throws.TypeOf<FailableSuccessConstructionException>() );
      }

      [Test]
      public void ConstructSuccess_NotNull()
      {
         var testValue = Failable<object, object>.Success( new object() );

         Assert.That( testValue.IsSuccess, Is.True );
         Assert.That( testValue.IsFailure, Is.False );
         Assert.That( testValue.Enum, Is.EqualTo( FlbType.Success ) );
      }

      [Test]
      public void ConstructFailure_ErrorNull()
      {
         Assert.That( () => Failable<object, object>.Failure( null ), Throws.TypeOf<FailableFailureConstructionException>() );
      }

      [Test]
      public void ConstructFailure_ErrorNotNull()
      {
         var testValue = Failable<object, object>.Failure( new object() );

         Assert.That( testValue.IsSuccess, Is.False );
         Assert.That( testValue.IsFailure, Is.True );
      }

      [Test]
      [Description( "Kann erfolgreiche Failables vom Wertetyp 'Exception' konstruieren." )]
      public void ConstructSuccess_ValueIsAnException()
      {
         //WICHTIG: Es gibt Argumente, das hier zu verbieten und analog zum Failable auf ein Failure zu mappen
         //         Es gibt aber auch Argumente, es so zu lassen wie es ist. Aktuell wurde sich für die
         //         weniger invasive Variante entschieden, vor allem da es weniger implizite Sachen macht.
         //         Dieser Test dient zur Dokumentation dieses Verhaltens.
         var x = Failable<Exception, string>.Success( new Exception() );

         Assert.That( x.IsSuccess, Is.True );
         Assert.That( x.IsFailure, Is.False );
      }

      [Test]
      [Description( "Nesting-Ebenen werden beachtet." )]
      public void Nesting()
      {
         var successInnerSuccess = Failable<Failable<int, string>, string>.Success( Failable<int, string>.Success( 123 ) );
         var successInnerFail = Failable<Failable<int, string>, string>.Success( Failable<int, string>.Failure( "hallo" ) );
         var failureInnerSuccess = Failable<int, Failable<int, string>>.Failure( Failable<int, string>.Success( 123 ) );
         var failureInnerFail = Failable<int, Failable<int, string>>.Failure( Failable<int, string>.Failure( "hallo" ) );

         Assert.That( successInnerSuccess.IsSuccess, Is.True );
         Assert.That( successInnerFail.IsSuccess, Is.True );
         Assert.That( successInnerSuccess.OrThrow().IsSuccess, Is.True );
         Assert.That( successInnerFail.OrThrow().IsSuccess, Is.False );
         Assert.That( failureInnerSuccess.IsSuccess, Is.False );
         Assert.That( failureInnerFail.IsSuccess, Is.False );
         Assert.That( failureInnerSuccess.FailureOrThrow().IsSuccess, Is.True );
         Assert.That( failureInnerFail.FailureOrThrow().IsSuccess, Is.False );
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
      public void ToString_Success()
      {
         Assert.That( Failable<TestObj, String>.Success( new TestObj( "hallo" ) ).ToString(), Is.EqualTo( "[Failable<TestObj, String>.Success: Object 'hallo']" ) );
      }

      [Test]
      public void ToString_Failure()
      {
         Assert.That( Failable<String, TestObj>.Failure( new TestObj( "evil" ) ).ToString(), Is.EqualTo( "[Failable<String, TestObj>.Failure: Object 'evil']" ) );
      }

      #endregion ToString

      #region Equals

      [Test]
      [Description( "Beide haben denselben Wert." )]
      public void Equals_SuccessEqual()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, int>( "abc", 123 );

         var x = Failable<Tuple<string, int>, string>.Success( t1 );
         var y = Failable<Tuple<string, int>, string>.Success( t2 );

         Assert.That( t1.Equals( t2 ), Is.True );
         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Beide haben Werte, aber unterschiedlich." )]
      public void Equals_SuccessInequal()
      {
         var x = Failable<int, string>.Success( 4 );
         var y = Failable<int, string>.Success( 5 );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Beide haben denselben Fehler." )]
      public void Equals_FailedEqual()
      {
         var x = Failable<object, int>.Failure( 123 );
         var y = Failable<object, int>.Failure( 123 );

         Assert.That( x.Equals( y ), Is.True );
      }

      [Test]
      [Description( "Es ist NICHT egal, welches Problem ein gescheitertes Failable hat" )]
      public void Equals_FailedInequal()
      {
         var x = Failable<object, string>.Failure( "what a disappointment" );
         var y = Failable<object, string>.Failure( "yet another disappointment" );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Gescheitertes Failable ist ungleich jedem Erfolgreichen." )]
      public void Equals_FailedInequalToSuccess()
      {
         var x = Failable<int, string>.Failure( "how appaling!" );
         var y = Failable<int, string>.Success( 0 );
         var z = Failable<Failable<int, string>, string>.Success( y );

         Assert.That( x.Equals( y ), Is.False );
         Assert.That( x.Equals( z ), Is.False );
      }

      [Test]
      [Description( "Failables zu unterschiedlichen Typen sind unterschiedlich." )]
      public void Equals_DifferentTypeInequal()
      {
         var t1 = new Tuple<string, int>( "abc", 123 );
         var t2 = new Tuple<string, string>( "abc", "123" );

         var x = Failable<Tuple<string, int>, string>.Success( t1 );
         var y = Failable<Tuple<string, string>, string>.Success( t2 );

         Assert.That( t1.Equals( t2 ), Is.False );
         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Failables zu unterschiedlichen Typen sind unterschiedlich - für Failure wird keine Ausnahme gemacht." )]
      public void Equals_DifferentTypeInequalForFailure()
      {
         var x = Failable<int, string>.Failure( "horrible" );
         var y = Failable<object, string>.Failure( "horrible" );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Failables zu unterschiedlichen Typen sind unterschiedlich, auch bei Subtypen im Failure-Fall!" )]
      public void DifferentInheritedType_Failure()
      {
         var sameText = "ANY_VALUE";
         var a = new MyClassWithTypicalValueBasedEquals( sameText );
         var b = new MySubclassWithTypicalValueBasedEquals( sameText, 123 );
         Assert.That( a.Equals( b ), Is.True, "TEST-SETUP IS BROKEN!" );
         Assert.That( b.Equals( a ), Is.True, "TEST-SETUP IS BROKEN!" );

         var x = Failable<int, MyClassWithTypicalValueBasedEquals>.Failure( a );
         var y = Failable<int, MySubclassWithTypicalValueBasedEquals>.Failure( b );

         Assert.That( x.Equals( y ), Is.False );
      }

      [Test]
      [Description( "Failables zu unterschiedlichen Typen sind unterschiedlich, auch bei Subtypen im Success-Fall!" )]
      public void DifferentInheritedType_Success()
      {
         var sameText = "ANY_VALUE";
         var a = new MyClassWithTypicalValueBasedEquals( sameText );
         var b = new MySubclassWithTypicalValueBasedEquals( sameText, 123 );
         Assert.That( a.Equals( b ), Is.True, "TEST-SETUP IS BROKEN!" );
         Assert.That( b.Equals( a ), Is.True, "TEST-SETUP IS BROKEN!" );

         var x = Failable<MyClassWithTypicalValueBasedEquals, int>.Success( a );
         var y = Failable<MySubclassWithTypicalValueBasedEquals, int>.Success( b );

         Assert.That( x.Equals( y ), Is.False );
      }

      #endregion Equals

      #region Accessing Value

      [Test]
      public void Or_FailureOrNull()
      {
         Assert.That( Failable<object, int>.Failure( -1 ).Or( null ), Is.Null );
      }

      [Test]
      public void Or_FailureOrValue()
      {
         Assert.That( Failable<int, int>.Failure( -1 ).Or( 1 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void Or_Value()
      {
         Assert.That( Failable<int, int>.Success( 1 ).Or( 2 ), Is.EqualTo( 1 ) );
      }

      [Test]
      public void Or_Use_Value()
      {
         Assert.That( Failable<int, string>.Success( 1 ).OrUse( s => -Int32.Parse( s ) ), Is.EqualTo( 1 ) );
         Assert.That( Failable<int, string>.Failure( "1" ).OrUse( s => -Int32.Parse( s ) ), Is.EqualTo( -1 ) );
      }

      [Test]
      public void Error_ThrowsIfSuccess()
      {
         Assert.Throws<FailableNoFailureException>( () => Failable<string, int>.Success( "HAPPY" ).FailureOrThrow() );
      }

      [Test]
      public void Error_ReturnsFailure()
      {
         Assert.That( Failable<string, int>.Failure( -1 ).FailureOrThrow(), Is.EqualTo( -1 ) );
      }

      [Test]
      public void OrThrow_ThrowsIfFailure()
      {
         Assert.Throws<FailableFailureException>( () => Failable<string, int>.Failure( -1 ).OrThrow() );
      }

      [Test]
      public void OrThrow_DoesNotThrowIfSuccess()
      {
         var testValue = ""; //to be overwritten by "happy"

         Assert.DoesNotThrow( () => testValue = Failable<string, int>.Success( "HAPPY" ).OrThrow() );
         Assert.That( testValue, Is.EqualTo( "HAPPY" ) );
      }

      [Test]
      public void OrThrowWithText_DoesThrowIfFailure()
      {
         var f = Failable<string, int>.Failure( -1 );
         var excp = Assert.Throws<FailableFailureException>( () => f.OrThrow( "test" ) );
         Assert.That( excp.Message, Is.EqualTo( "test" ) );
      }

      [Test]
      public void OrThrowWithText_DoesNotThrowIfSuccess()
      {
         var testValue = 0;

         Assert.DoesNotThrow( () => testValue = Failable<int, int>.Success( 1 ).OrThrow( "Test" ) );
         Assert.That( testValue, Is.EqualTo( 1 ) );
      }

      #endregion Accessing Value

      #region Subtypes

      [Test]
      public void OrWithSubtype_Success()
      {
         var myClass = new MyClassWithTypeBasedEquals();
         var mySubclass = new MySubclassWithTypeBasedEquals();

         var underTest = Failable<MyClassWithTypeBasedEquals, Exception>.Success( myClass );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( myClass ) );
      }

      [Test]
      public void OrWithSubtype_Failure()
      {
         var mySubclass = new MySubclassWithTypeBasedEquals();

         var underTest = Failable<MyClassWithTypeBasedEquals, Exception>.Failure( new ArgumentException( "irrelevant" ) );
         var actualValue = underTest.Or( mySubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelType_Success()
      {
         var mySubclass = new MySubclassWithTypeBasedEquals();
         var myOtherSubclass = new MyOtherSubclassWithTypeBasedEquals();

         var underTest = Failable<MyClassWithTypeBasedEquals, Exception>.Success( mySubclass );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( mySubclass ) );
      }

      [Test]
      public void OrWithParallelType_Failure()
      {
         var myOtherSubclass = new MyOtherSubclassWithTypeBasedEquals();

         var underTest = Failable<MyClassWithTypeBasedEquals, Exception>.Failure( new ArgumentException( "irrelevant" ) );
         var actualValue = underTest.Or( myOtherSubclass );

         Assert.That( actualValue, Is.SameAs( myOtherSubclass ) );
      }

      #endregion Subtypes

      #region Flatten
      [Test]
      public void Flatten_Success()
      {
         var a = Failable<string, int>.Success( "hallo" );
         var b = Failable<Failable<string, int>, int>.Success( Failable<string, int>.Success( "hallo" ) );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.EqualTo( a ) );
      }

      [Test]
      public void Flatten_Failure()
      {
         var a = Failable<string, int>.Failure( 42 );
         var b = Failable<Failable<string, int>, int>.Success( Failable<string, int>.Failure( 42 ) );
         var c = Failable<Failable<string, int>, int>.Success( Failable<string, int>.Success( "whatever" ) );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.EqualTo( a ) );
         Assert.That( b.Flatten(), Is.Not.EqualTo( c ) );
      }

      [Test]
      public void Flatten_DifferentFailType_Success()
      {
         var a = Failable<string, int>.Success( "hallo" );
         var b = Failable<Failable<string, string>, int>.Success( Failable<string, string>.Success( "hallo" ) );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.EqualTo( a ) );
      }

      [Test]
      public void Flatten_DifferentFailType_Failure()
      {
         var a = Failable<string, int>.Failure( 42 );
         var b = Failable<Failable<string, string>, int>.Success( Failable<string, string>.Failure( "error" ) );
         var c = Failable<Failable<string, string>, int>.Success( Failable<string, string>.Success( "whatever" ) );

         Assert.That( b, Is.Not.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.EqualTo( a ) );
         Assert.That( b.Flatten( ( s ) => 42 ), Is.Not.EqualTo( c ) );
      }
      #endregion Flatten

      #region Map and Nesting
      [Test]
      [Description( "Die Map-Operation wird auf Erfolge angewendet." )]
      public void Map_Success()
      {
         var original = Failable<string, bool>.Success( "hallo" );
         var result = original.Map( s => s += " welt" );
         Assert.That( result.OrThrow, Is.EqualTo( "hallo welt" ) );
      }

      [Test]
      [Description( "Die Map-Operation wird nicht auf Fehlschläge angewendet." )]
      public void Map_Failure()
      {
         var original = Failable<string, bool>.Failure( false );
         var result = original.Map( s => s += " welt" );
         Assert.That( result.FailureOrThrow, Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Die Map-Operation wird nicht auf Fehlschläge angewendet." )]
      public void Map_Failure_SideEffect()
      {
         var myInt = 0;
         var original = Failable<string, bool>.Failure( false );
         var result = original.Map( s => ++myInt );
         Assert.That( myInt, Is.EqualTo( 0 ) );
      }

      [Test]
      [Description( "Die Map-Operation ist kompatibel mit Verschachtelung" )]
      public void Map_NestingInMap()
      {
         var flag = Failable<bool, string>.Success( true );

         var result = flag.Map( _ => Failable<bool, string>.Success( true ) );

         Assert.That( result.Or( Failable<bool, string>.Failure( "disgusting" ) ).Or( false ), Is.True );
      }

      [Test]
      [Description( "Map hat keine Probleme mit Typveränderung, weder zur Lauf- noch zur Compilezeit." )]
      public void MapToDifferentType()
      {
         var one = Failable<int, bool>.Success( 1 );
         Failable<string, bool> onePlusOne = one.Map( i => $"{i}+1=2" );

         Assert.That( onePlusOne.OrThrow(), Is.EqualTo( "1+1=2" ) );
      }

      [Test]
      [Description( "FlatMap hat keine Probleme mit Typveränderung, weder zur Lauf- noch zur Compilezeit." )]
      public void FlatMapToDifferentType()
      {
         var one = Failable<int, bool>.Success( 1 );
         Failable<string, bool> onePlusOne = one.FlatMap( i => Failable<string, bool>.Success( $"{i}+1=2" ) );

         Assert.That( onePlusOne.OrThrow(), Is.EqualTo( "1+1=2" ) );
      }

      [Test]
      [Description( "Die MapFailure-Operation wird auf Fehlschläge angewendet." )]
      public void MapFailure_Failure()
      {
         var original = Failable<bool, string>.Failure( "hallo" );
         var result = original.MapFailure( s => s += " welt" );
         Assert.That( result.FailureOrThrow(), Is.EqualTo( "hallo welt" ) );
      }

      [Test]
      [Description( "Die MapFailure-Operation wird nicht auf Erfolge angewendet." )]
      public void MapFailure_Success()
      {
         var original = Failable<bool, string>.Success( false );
         var result = original.MapFailure( s => s += " welt" );
         Assert.That( result.OrThrow(), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Die MapFailure-Operation wird nicht auf Erfolge angewendet." )]
      public void MapFailure_Success_SideEffect()
      {
         var myInt = 0;
         var original = Failable<bool, string>.Success( false );
         var result = original.MapFailure( s => ++myInt );
         Assert.That( myInt, Is.EqualTo( 0 ) );
      }

      [Test]
      [Description( "MapFailure hat keine Probleme mit Typveränderung, weder zur Lauf- noch zur Compilezeit." )]
      public void MapFailureToDifferentType()
      {
         var one = Failable<bool, int>.Failure( 1 );
         Failable<bool, string> onePlusOne = one.MapFailure( i => $"{i}+1=2" );

         Assert.That( onePlusOne.FailureOrThrow(), Is.EqualTo( "1+1=2" ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit FlatMap." )]
      public void Map_NestingInFlatMap()
      {
         var flag1 = Failable<int, string>.Success( 1 );
         var flag2 = Failable<int, string>.Success( 2 );

         var result = flag1.FlatMap( outerInt => flag2.Map( innerInt => outerInt + innerInt ) );

         Assert.That( result.Or( -999 ), Is.EqualTo( 3 ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit FlatMap und Option." )]
      public void Map_NestingInFlatMap_FlattenToOption()
      {
         var flag1 = Failable<int, string>.Success( 1 );
         var flag2 = Option<int, string>.Some( 2 );

         var result = flag1.FlatMap( outerInt => flag2.Map( innerInt => outerInt + innerInt ) );

         Assert.True( result.IsSome );
         Assert.That( result.Or( -999 ), Is.EqualTo( 3 ) );
      }

      [Test]
      [Description( "Verschachtelte Fallunterscheidungen mit Wertübergabe mit FlatMap." )]
      public void ValuePropagationInFlatMap()
      {
         var hallo = Failable<string, int>.Success( "hallo" );
         var sp = Failable<string, int>.Success( " " );

         var result = hallo.FlatMap( h => sp.Map( space => h + space + "welt" ) );

         Assert.That( result.Or( "nix da" ), Is.EqualTo( "hallo welt" ) );
      }

      #endregion Map and Nesting

      #region Is

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'true'" )]
      public void ContainsDefinedTrue()
      {
         var f = Failable<string, Exception>.Success( "hubba" );

         Assert.That( f.Is( "hubba" ), Is.EqualTo( true ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - Vergleich liefert 'false'" )]
      public void ContainsDefinedFalse()
      {
         var f = Failable<string, Exception>.Success( "hubba" );

         Assert.That( f.Is( "whatever" ), Is.EqualTo( false ) );
      }

      [Test]
      [Description( "Check mittels Direktvergleich - None immer 'false'" )]
      public void ContainsFalseWhenNone()
      {
         var f = Failable<string, Exception>.Failure( new ArgumentException() );

         Assert.That( f.Is( "hubba" ), Is.EqualTo( false ) );
      }

      #endregion Is

      #region Holds

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'true'" )]
      public void Is_PredicateMatchingDefined()
      {
         var failable = Failable<string, Exception>.Success( "hubba" );

         Assert.That( failable.Holds( s => s.Equals( "hubba" ) ), Is.EqualTo( true ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich liefert 'false'" )]
      public void Is_PredicateNonMatchingDefined()
      {
         var failable = Failable<string, Exception>.Success( "hubba" );

         Assert.That( failable.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      [Test]
      [Description( "Check mittels Predikatsfunktion - Vergleich findet nicht statt" )]
      public void Is_PredicateUndefined()
      {
         var failable = Failable<string, Exception>.Failure( new ArgumentException() );

         Assert.That( failable.Holds( s => s.Equals( "hubba-hub" ) ), Is.EqualTo( false ) );
                                                   
      }

      #endregion Holds

      #region TryGet

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei Failure" )]
      public void TryGet_Result_Failure()
      {
         var failable = Failable<string, Exception>.Failure( new ArgumentException() );

         Assert.IsFalse( failable.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei Failure" )]
      public void TryGet_Value_Failure()
      {
         var failable = Failable<string, Exception>.Failure( new ArgumentException() );
         failable.TryGetValue( out var s );

         Assert.IsNull( s );
      }

      [Test]
      [Description( "TryGetValue() produziert korrektes Boolean-Result bei Success" )]
      public void TryGet_Value_Success()
      {
         var failable = Failable<string, Exception>.Success( "blub" );

         Assert.IsTrue( failable.TryGetValue( out var s ) );
      }

      [Test]
      [Description( "TryGetValue() schreibt keinen Wert bei Success" )]
      public void TryGet_Result_Success()
      {
         var failable = Failable<string, Exception>.Success( "blub" );
         failable.TryGetValue( out var s );

         Assert.That( s, Is.EqualTo( "blub" ) );
      }

      #endregion TryGet

      #region TryGetFailure

      [Test]
      [Description( "TryGetFailure() produziert korrektes Boolean-Result bei Success" )]
      public void TryGet_Error_Success()
      {
         var failable = Failable<string, Exception>.Success( "blah" );

         Assert.IsFalse( failable.TryGetFailure( out var s ) );
      }

      [Test]
      [Description( "TryGetFailure() schreibt keinen Wert bei Success" )]
      public void TryGet_ErrorResult_Success()
      {
         var failable = Failable<string, Exception>.Success( "blah" );
         failable.TryGetFailure( out var s );

         Assert.IsNull( s );
      }

      [Test]
      [Description( "TryGetFailure() produziert korrektes Boolean-Result bei Failure" )]
      public void TryGet_Error_Failure()
      {
         var failable = Failable<string, Exception>.Failure( new ArgumentException() );

         Assert.IsTrue( failable.TryGetFailure( out var s ) );
      }

      [Test]
      [Description( "TryGetFailure() schreibt Wert bei Failure" )]
      public void TryGet_ErrorResult_Failure()
      {
         var failable = Failable<string, Exception>.Failure( new ArgumentException("msg") );
         failable.TryGetFailure( out var e );

         Assert.That( e, Is.TypeOf<ArgumentException>() );
         Assert.That( e.Message, Is.EqualTo("msg") );
      }

      #endregion TryGetFailure

      #region Convert
      
      [Test]
      [Description( "Success wird zu Maybe.Some konvertiert" )]
      public void ConvertToMaybe_Success()
      {
         var failable = Failable<string, Exception>.Success( "hallo" );
         var maybe = failable.ToMaybe();

         Assert.That( maybe.IsSome, Is.True );
         Assert.That( maybe.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Failure wird zu Maybe.None konvertiert" )]
      public void ConvertToMaybe_Failure()
      {
         var failable = Failable<string, Exception>.Failure( new ArgumentException() );
         var maybe = failable.ToMaybe();

         Assert.That( maybe.IsNone, Is.True );
      }

      [Test]
      [Description( "Failure wird zu Option.Failure konvertiert" )]
      public void ConvertToOption_None()
      {
         var failable = Failable<string, int>.Failure( 42 );
         var option = failable.ToOption();

         Assert.That( option.IsFailure, Is.True );
      }

      [Test]
      [Description( "Success wird zu Option.Some konvertiert" )]
      public void ConvertToOption_Some()
      {
         var failable = Failable<string, int>.Success( "hubba" );
         var option = failable.ToOption();

         Assert.That( option.IsSome, Is.True );
      }

      [Test]
      [Description( "Failure wird zu Option.Failure konvertiert" )]
      public void ConvertToOption_SwitchValueType_None()
      {
         var failable = Failable<string, int>.Failure( 42 );
         var option = failable.ToOptionWith( (s) => true );

         Assert.That( option.IsFailure, Is.True );
      }

      [Test]
      [Description( "Success wird zu Option.Some konvertiert" )]
      public void ConvertToOption_SwitchValueType_Some()
      {
         var failable = Failable<string, int>.Success( "hubba" );
         var option = failable.ToOptionWith( ( s ) => true );

         Assert.That( option.IsSome, Is.True );
         Assert.IsTrue( option.OrThrow() );
      }

      [Test]
      [Description( "Success wird zu EFailable.Success konvertiert" )]
      public void ConvertToEFailable_Success()
      {
         var failable = Failable<string, bool>.Success( "hallo" );
         var EFailable = failable.ToEFailable( new ArgumentException() );

         Assert.That( EFailable.IsSuccess, Is.True );
         Assert.That( EFailable.OrThrow, Is.EqualTo( "hallo" ) );
      }

      [Test]
      [Description( "Failure wird zu EFailable.Failure konvertiert" )]
      public void ConvertToEFailable_Failure()
      {
         var failable = Failable<string, bool>.Failure( true );
         EFailable<string> EFailable = failable.ToEFailable( new ArgumentException( "msg" ) );

         Assert.That( EFailable.IsFailure, Is.True );
         Assert.That( EFailable.FailureOrThrow().Message, Is.EqualTo( "msg" ) );
      }

      [Test]
      [Description( "Success wird zu EFailable.Success konvertiert" )]
      public void ConvertToEFailable_WithConverter_Success()
      {
         var failable = Failable<int, string>.Success( 42 );
         var eFailable = failable.ToEFailableWith( i => $"1{i}", new Exception( "whatever" ) );

         Assert.That( eFailable.IsSuccess, Is.True );
         Assert.That( eFailable.OrThrow(), Is.EqualTo( "142" ) );
      }

      [Test]
      [Description( "Failure wird zu EFailable.Failure konvertiert" )]
      public void ConvertToEFailable_WithConverter_Failure()
      {
         var failable = Failable<int, string>.Failure( "will-be-overwritten" );
         var eFailable = failable.ToEFailableWith( i => $"1{i}", new Exception( "new" ) );

         Assert.That( eFailable.IsFailure, Is.True );
         Assert.That( eFailable.FailureOrThrow().Message, Is.EqualTo( "new" ) );
      }
      #endregion Convert
   }
}