namespace qs.Extensions.Int32Extensions
{
    using System;

    //using System.Numeric;
    public static class Int32Extensions
    {
        #region Methods

        /// <summary>
        /// Compares this instance with another instance of nullable System.Int32.
        /// </summary>
        /// <param path="a">Instance of type nullable System.Int32 to use in the comparison.</param>
        /// <param path="a">A comparand of type nullable System.Int32.</param>
        /// <returns>A 32-bit signed integer indicating the lexical relationship between the two comparands. Value Condition Less than zero This instance is less than a -or- this instance is null. Zero This instance is equal to a -or- this instance and a are both null. Greater than zero This instance is greater than a -or- a is null.</returns>
        public static int CompareTo(this int? a, int? b)
        {
            if (a == null && b == null)
                return 0;
            if (a == null)
                return -1;
            if (b == null)
                return 1;
            if (a == b)
                return 0;
            if (a < b)
                return -1;
            if (a > b)
                return 1;
            return 0;
        }

        /// <summary>
        /// Performs the specified action X times.
        /// Action encapsulates a method that accepts one parameter, the counter as an Int32, and does not return a value.
        /// Example:
        /// int j = 5;
        /// j.TimesDo( t =>
        /// {
        ///     Console.WriteLine( t );
        /// } );
        /// Output:
        /// > 0
        /// > 1
        /// > 2
        /// > 3
        /// > 4
        /// Example:
        /// 5.TimesDo( t =>
        /// {
        ///     Console.WriteLine( t );
        /// } );
        /// Output:
        /// > 0
        /// > 1
        /// > 2
        /// > 3
        /// > 4
        /// Example:
        /// int j = 5;
        /// j.TimesDo( new Action&lt;int&gt;( testmethod ) );
        /// ...
        /// private void testmethod( int x )
        /// {
        ///     Console.WriteLine( x );
        /// }
        /// Output:
        /// > 0
        /// > 1
        /// > 2
        /// > 3
        /// > 4
        /// Example:
        /// int j = 5;
        /// j.TimesDo( testmethod );
        /// ...
        /// private void testmethod( int x )
        /// {
        ///     Console.WriteLine( x );
        /// }
        /// Output:
        /// > 0
        /// > 1
        /// > 2
        /// > 3
        /// > 4
        /// </summary>
        /// <param name="a"></param>
        /// <param name="action">Encapsulates a method that accepts one parameter, the counter as an Int32.</param>
        public static void TimesDo(this int a, Action<int> action)
        {
            if (action == null)
                return;
            for (int i = 0; i <= a - 1; i++)
                action(i);
        }

        /// <summary>
        /// Performs the specified action beginning from the base and iterating to the end including the end.
        /// Action encapsulates a method that accepts two parameters, the counter as an Int32 and the end as an Int32, and does not return a value.
        /// Example:
        /// 5.To( 10, ( counter, end ) =>
        /// {
        ///     Console.WriteLine( "Counter=" + counter + " End=" + end );
        /// } );
        /// Output:
        /// > Counter=5 End=10
        /// > Counter=6 End=10
        /// > Counter=7 End=10
        /// > Counter=8 End=10
        /// > Counter=9 End=10
        /// > Counter=10 End=10
        /// Example:
        /// 5.To( 10, ( counter, end ) =>
        /// {
        ///     Console.WriteLine( "Counter=" + counter + " End=" + end );
        /// } );
        /// Output:
        /// > Counter=5 End=10
        /// > Counter=6 End=10
        /// > Counter=7 End=10
        /// > Counter=8 End=10
        /// > Counter=9 End=10
        /// > Counter=10 End=10
        /// Example:
        /// 5.To( 10, new Action&lt;int, int&gt;( testmethod ) );
        /// ...
        /// private void testmethod( int x, int y )
        /// {
        ///     Console.WriteLine( "Counter=" + x + " End=" + y );
        /// }
        /// Output:
        /// > Counter=5 End=10
        /// > Counter=6 End=10
        /// > Counter=7 End=10
        /// > Counter=8 End=10
        /// > Counter=9 End=10
        /// > Counter=10 End=10
        /// Example:
        /// 5.To( 10, testmethod );
        /// ...
        /// private void testmethod( int x, int y )
        /// {
        ///     Console.WriteLine( "Counter=" + x + " End=" + y );
        /// }
        /// Output:
        /// > Counter=5 End=10
        /// > Counter=6 End=10
        /// > Counter=7 End=10
        /// > Counter=8 End=10
        /// > Counter=9 End=10
        /// > Counter=10 End=10
        /// </summary>
        /// <param name="a"></param>
        /// <param name="end">Specifies the end of the counter.</param>
        /// <param name="action">Encapsulates a method that accepts two parameters, the counter as an Int32 and the end as an Int32, and does not return a value.</param>
        public static void To(this int a, int end, Action<int, int> action)
        {
            if (action == null)
                return;
            for (int i = a; i <= end; i++)
                action(i, end);
        }

        /// <summary>
        /// Returns the absolute value of a specified Int32.
        /// </summary>
        /// <param name="a">Instance of type System.Int32.</param>
        /// <returns>Returns tha absolute value.</returns>
        public static int ToAbs(this int a)
        {
            return Math.Abs(a);
        }

        /// <summary>
        /// Convert the nullable Int32 to a nullable System.Double.
        /// </summary>
        /// <param name="a">Instance of type nullable System.Int32 to be converted.</param>
        /// <returns>Returns a nullable System.Double.</returns>
        public static double? ToDouble(this int? a)
        {
            return a.HasValue ? Convert.ToDouble(a) : new double?();
        }

        /// <summary>
        /// Convert the Int32 to a System.Double.
        /// </summary>
        /// <param name="a">Instance of type System.Int32 to be converted.</param>
        /// <returns>Returns a System.Double.</returns>
        public static double ToDouble(this int a)
        {
            return Convert.ToDouble(a);
        }

        /// <summary>
        /// Convert the Int32 to a System.Single.
        /// </summary>
        /// <param name="a">Instance of type System.Int32</param>
        /// <returns>Returns a System.Single.</returns>
        public static float ToFloat(this int a)
        {
            return (float)a;
        }

        /// <summary>
        /// Convert the Int32 to its corresponding string hexadecimal representation.
        /// </summary>
        /// <param name="a">Instance of type System.Int32 to be converted.</param>
        /// <returns>Returns as a System.String the corresponding hexadecimal representation.</returns>
        public static string ToHex(this int a)
        {
            return a.ToString("x");
        }

        /// <summary>
        /// Convert the Int32 to the equivalent Roman Numeral.
        /// </summary>
        /// <param name="a">Instance of type System.Int32 to be converted.</param>
        /// <returns>Returns an instance of Roman Numeral.</returns>
        public static RomanNumeral ToRomanNumeral(this int a)
        {
            return new RomanNumeral(a);
        }

        /// <summary>
        /// Convert the Int32 to a System.Single.
        /// </summary>
        /// <param name="a">Instance of type System.Int32</param>
        /// <returns>Returns a System.Single.</returns>
        public static float ToSingle(this int a)
        {
            return (float)a;
        }

        /// <summary>
        /// Performs the specified action beginning from the base and interating to the end, but not including the end.
        /// Action encapsulates a method that accepts two parameters, the counter as an Int32 and the end as an Int32, and does not return a value.
        /// Example:
        /// 5.UpTo( 10, ( counter, end ) =>
        /// {
        ///     Console.WriteLine( "Counter=" + counter + " End=" + end );
        /// } );
        /// Output:
        /// > Counter=5 End=10
        /// > Counter=6 End=10
        /// > Counter=7 End=10
        /// > Counter=8 End=10
        /// > Counter=9 End=10
        /// Example:
        /// 5.UpTo( 10, ( counter, end ) =>
        /// {
        ///     Console.WriteLine( "Counter=" + counter + " End=" + end );
        /// } );
        /// Output:
        /// > Counter=5 End=10
        /// > Counter=6 End=10
        /// > Counter=7 End=10
        /// > Counter=8 End=10
        /// > Counter=9 End=10
        /// Example:
        /// 5.UpTo( 10, new Action&lt;int, int&gt;( testmethod ) );
        /// ...
        /// private void testmethod( int x, int y )
        /// {
        ///     Console.WriteLine( "Counter=" + x + " End=" + y );
        /// }
        /// Output:
        /// > Counter=5 End=10
        /// > Counter=6 End=10
        /// > Counter=7 End=10
        /// > Counter=8 End=10
        /// > Counter=9 End=10
        /// Example:
        /// 5.UpTo( 10, testmethod );
        /// ...
        /// private void testmethod( int x, int y )
        /// {
        ///     Console.WriteLine( "Counter=" + x + " End=" + y );
        /// }
        /// Output:
        /// > Counter=5 End=10
        /// > Counter=6 End=10
        /// > Counter=7 End=10
        /// > Counter=8 End=10
        /// > Counter=9 End=10
        /// </summary>
        /// <param name="a"></param>
        /// <param name="end">Specifies the end of the counter.</param>
        /// <param name="action">Encapsulates a method that accepts two parameters, the counter as an Int32 and the end as an Int32, and does not return a value.</param>
        public static void UpTo(this int a, int end, Action<int, int> action)
        {
            if (action == null)
                return;
            for (int i = a; i <= end - 1; i++)
                action(i, end);
        }

        #endregion Methods
    }
}