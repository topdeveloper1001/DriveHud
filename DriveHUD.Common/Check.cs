﻿// Provides support for Design By Contract
// as described by Bertrand Meyer in his seminal book,
// Object-Oriented Software Construction (2nd Ed) Prentice Hall 1997
// (See chapters 11 and 12).
//
// See also Building Bug-free O-O Software: An Introduction to Design by Contract
// http://www.eiffel.com/doc/manuals/technology/contract/
// 
// The following conditional compilation symbols are supported:
// 
// These suggestions are based on Bertrand Meyer's Object-Oriented Software Construction (2nd Ed) p393
// 
// DBC_CHECK_ALL           - Check assertions - implies checking preconditions, postconditions and invariants
// DBC_CHECK_INVARIANT     - Check invariants - implies checking preconditions and postconditions
// DBC_CHECK_POSTCONDITION - Check postconditions - implies checking preconditions 
// DBC_CHECK_PRECONDITION  - Check preconditions only, e.g., in Release build
// 
// A suggested default usage scenario is the following:
// 
//#if DEBUG
//#define DBC_CHECK_ALL
//#else
//#define DBC_CHECK_PRECONDITION
//#endif

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace DriveHUD.Common
{
    /// <summary>
    /// Design By Contract Checks.
    /// 
    /// Each method generates an exception or
    /// a trace assertion statement if the contract is broken.
    /// </summary>
    /// <remarks>
    /// This example shows how to call the Require method.
    /// Assume DBC_CHECK_PRECONDITION is defined.
    /// <code>
    /// public void Test(int x)
    /// {
    /// 	try
    /// 	{
    ///			Check.Require(x > 1, "x must be > 1");
    ///		}
    ///		catch (System.Exception ex)
    ///		{
    ///			Console.WriteLine(ex.ToString());
    ///		}
    ///	}
    /// </code>
    /// If you wish to use trace assertion statements, intended for Debug scenarios,
    /// rather than exception handling then set 
    /// 
    /// <code>Check.UseAssertions = true</code>
    /// 
    /// You can specify this in your application entry point and maybe make it
    /// dependent on conditional compilation flags or configuration file settings, e.g.,
    /// <code>
    /// #if DBC_USE_ASSERTIONS
    /// Check.UseAssertions = true;
    /// #endif
    /// </code>
    /// You can direct output to a Trace listener. For example, you could insert
    /// <code>
    /// Trace.Listeners.Clear();
    /// Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
    /// </code>
    /// 
    /// or direct output to a file or the Event Log.
    /// 
    /// (Note: For ASP.NET clients use the Listeners collection
    /// of the Debug, not the Trace, object and, for a Release build, only exception-handling
    /// is possible.)
    /// </remarks>
    /// 
    public sealed class Check
    {
        #region Interface

        /// <summary>
        /// Precondition check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT"),
        Conditional("DBC_CHECK_POSTCONDITION"),
        Conditional("DBC_CHECK_PRECONDITION")]
        public static void Require(bool assertion, string message)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PreconditionException(message);
            }
            else
            {
                Trace.Assert(assertion, "Precondition: " + message);
            }
        }

        /// <summary>
        /// Precondition check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT"),
        Conditional("DBC_CHECK_POSTCONDITION"),
        Conditional("DBC_CHECK_PRECONDITION")]
        public static void Require(bool assertion, string message, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PreconditionException(message, inner);
            }
            else
            {
                Trace.Assert(assertion, "Precondition: " + message);
            }
        }

        private static Exception GetException<TException>(string message, string str)
        {
            Exception inner = null;
            var constructor = typeof(TException).GetConstructor(new [] { typeof(string), typeof(string) });

            if (constructor != null)
            {
                if (constructor.GetParameters()[0].Name == "paramName")
                {
                    inner = constructor.Invoke(new object[] { message, str }) as Exception;
                }
                else
                {
                    inner = constructor.Invoke(new object[] { str, message }) as Exception;
                }
            }
            else
            {
                constructor = typeof(TException).GetConstructor(new [] { typeof(string) });

                if (constructor != null)
                {
                    inner = constructor.Invoke(new object[] { str }) as Exception;
                }
                else
                {
                    constructor = typeof (TException).GetConstructor(new Type[] {});

                    if (constructor != null)
                    {
                        inner = constructor.Invoke(new object[] { }) as Exception;
                    }
                }
            }

            if (inner == null)
            {
                throw new ArgumentException(str, message);
            }
            return inner;
        }

        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT"),
        Conditional("DBC_CHECK_POSTCONDITION"),
        Conditional("DBC_CHECK_PRECONDITION")]
        public static void Requires<TException>(bool assertion, string message)
            where TException: Exception
        {
            if (UseExceptions)
            {
                var inner = GetException<TException>(message, string.Empty);

                if (!assertion) throw new PreconditionException(message, inner);
            }
            else
            {
                Trace.Assert(assertion, "Precondition: " + message);
            }
        }

        /// <summary>
        /// Precondition check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT"),
        Conditional("DBC_CHECK_POSTCONDITION"),
        Conditional("DBC_CHECK_PRECONDITION")]
        public static void Require(bool assertion)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PreconditionException("Precondition failed.");
            }
            else
            {
                Trace.Assert(assertion, "Precondition failed.");
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT"),
        Conditional("DBC_CHECK_POSTCONDITION")]
        public static void Ensure(bool assertion, string message)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PostconditionException(message);
            }
            else
            {
                Trace.Assert(assertion, "Postcondition: " + message);
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT"),
        Conditional("DBC_CHECK_POSTCONDITION")]
        public static void Ensure(bool assertion, string message, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PostconditionException(message, inner);
            }
            else
            {
                Trace.Assert(assertion, "Postcondition: " + message);
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT"),
        Conditional("DBC_CHECK_POSTCONDITION")]
        public static void Ensure(bool assertion)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new PostconditionException("Postcondition failed.");
            }
            else
            {
                Trace.Assert(assertion, "Postcondition failed.");
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT")]
        public static void Invariant(bool assertion, string message)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new InvariantException(message);
            }
            else
            {
                Trace.Assert(assertion, "Invariant: " + message);
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT")]
        public static void Invariant(bool assertion, string message, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new InvariantException(message, inner);
            }
            else
            {
                Trace.Assert(assertion, "Invariant: " + message);
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL"),
        Conditional("DBC_CHECK_INVARIANT")]
        public static void Invariant(bool assertion)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new InvariantException("Invariant failed.");
            }
            else
            {
                Trace.Assert(assertion, "Invariant failed.");
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL")]
        public static void Assert(bool assertion, string message)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new AssertionException(message);
            }
            else
            {
                Trace.Assert(assertion, "Assertion: " + message);
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL")]
        public static void Assert(bool assertion, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new AssertionException(string.Empty, inner);
            }
            else
            {
                Trace.Assert(assertion, string.Empty);
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL")]
        public static void Assert(bool assertion, string message, Exception inner)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new AssertionException(message, inner);
            }
            else
            {
                Trace.Assert(assertion, "Assertion: " + message);
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL")]
        public static void Assert<TException>(bool assertion, string message)
            where TException : Exception
        {
            if (UseExceptions)
            {
                var inner = Check.GetException<TException>(message, string.Empty);

                if (!assertion) throw new AssertionException(message, inner);
            }
            else
            {
                Trace.Assert(assertion, "Assertion: " + message);
            }
        }


        /// <summary>
        /// Assertion check.
        /// </summary>
        [Conditional("DBC_CHECK_ALL")]
        public static void Assert(bool assertion)
        {
            if (UseExceptions)
            {
                if (!assertion) throw new AssertionException("Assertion failed.");
            }
            else
            {
                Trace.Assert(assertion, "Assertion failed.");
            }
        }

        /// <summary>
        /// Assert argument not null
        /// </summary>
        /// <example>
        /// void SavePerson(Person person)
        /// {
        ///     Check.ArgumentNotNull(() => person);
        ///     //...
        /// }
        /// </example>
        /// <param name="argumentExpression">lambda expression capturing argument: () => arg</param>
        [Conditional("DBC_CHECK_ALL")]
        public static void ArgumentNotNull<T>(Expression<Func<T>> argumentExpression)
            where T : class
        {
            var memberExp = argumentExpression.Body as MemberExpression;

            if (memberExp == null)
                throw new ArgumentException("Invalid Contract: ArgumentExpression was not a MemberExpression.");

            var constantExpression = memberExp.Expression as ConstantExpression;

            if (constantExpression == null)
                throw new ArgumentException("Invalid Contract: ArgumentExpression didn't contain a ConstantExpression.");

            // Argument will be a field on the class.  
            var fieldInfo = memberExp.Member as System.Reflection.FieldInfo;
            // The contant expression will contain the object we're calling from.  
            var methodOwner = constantExpression.Value;

            // Use the fieldInfo to extract the information directly from the owner  
            if (fieldInfo != null && fieldInfo.GetValue(methodOwner) == null)
                throw new ArgumentNullException(memberExp.Member.Name);
        }

        /// <summary>
        /// Set this if you wish to use Trace Assert statements 
        /// instead of exception handling. 
        /// (The Check class uses exception handling by default.)
        /// </summary>
        public static bool UseAssertions
        {
            get
            {
                return useAssertions;
            }
            set
            {
                useAssertions = value;
            }
        }

        #endregion // Interface

        #region Implementation

        // No creation
        private Check() { }

        /// <summary>
        /// Is exception handling being used?
        /// </summary>
        private static bool UseExceptions
        {
            get
            {
                return !useAssertions;
            }
        }

        // Are trace assertion statements being used? 
        // Default is to use exception handling.
        private static bool useAssertions = false;

        #endregion // Implementation

    } // End Check

    #region Exceptions

    /// <summary>
    /// Exception raised when a contract is broken.
    /// Catch this exception type if you wish to differentiate between 
    /// any VI.Common exception and other runtime exceptions.
    ///  
    /// </summary>
    [Serializable]
    public class DesignByContractException : ApplicationException
    {
        protected DesignByContractException() { }
        protected DesignByContractException(string message) : base(message) { }
        protected DesignByContractException(string message, Exception inner) : base(message, inner) { }
        protected DesignByContractException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    /// <summary>
    /// Exception raised when a precondition fails.
    /// </summary>
    [Serializable]
    public class PreconditionException : DesignByContractException
    {
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException() { }
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message) : base(message) { }
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException(string message, Exception inner) : base(message, inner) { }
        protected PreconditionException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    /// <summary>
    /// Exception raised when a postcondition fails.
    /// </summary>
    [Serializable]
    public class PostconditionException : DesignByContractException
    {
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException() { }
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException(string message) : base(message) { }
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException(string message, Exception inner) : base(message, inner) { }
        protected PostconditionException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    /// <summary>
    /// Exception raised when an invariant fails.
    /// </summary>
    [Serializable]
    public class InvariantException : DesignByContractException
    {
        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException() { }
        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException(string message) : base(message) { }
        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException(string message, Exception inner) : base(message, inner) { }
        protected InvariantException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    /// <summary>
    /// Exception raised when an assertion fails.
    /// </summary>
    [Serializable]
    public class AssertionException : DesignByContractException
    {
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException() { }
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message) : base(message) { }
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException(string message, Exception inner) : base(message, inner) { }
        protected AssertionException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }

    #endregion // Exception classes

} // End Design By Contract