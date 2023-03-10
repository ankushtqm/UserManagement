#region namespaces

using System;

#endregion

namespace SusQTech.Data.DataObjects
{
    /// <summary>
    /// Abstract class to help create objects that don't throw exceptions, but hold them so processing can continue.
    /// In a try catch block the SetException should be used.
    /// </summary>
    /// <example>
    /// //Silly example:
    /// try {
    ///   ..some code here..
    /// } catch(Exception err) {
    ///    this.SetException(err);
    /// }
    /// if (this.HasException)
    ///   this.ThrowLastException()
    ///</example>
    ///<remarks>
    /// The idea of the exception holding object is that inheriting objects are essentially promising to
    /// catch all exceptions and use the SetException method.  Then all exception handling behavior can be
    /// handled via the OnException event, ThrowExceptions property, and the combination of HasException and
    /// LastException.  This allows processing on a page to continue without blowing up (say during an import)
    /// and the exceptions can be checked and reported later.
    ///</remarks>
    public abstract class ExceptionHoldingObject
    {
        #region private members

        private Exception _lastException = null;
        private bool _throwExceptions = false;

        #endregion

        /// <summary>
        /// Sets the LastException property, making HasException true.  Fires the OnException event.
        /// Re-throws the exception if ThrowExceptions is true, after firing the OnException event.
        /// </summary>
        /// <param name="err">The exception object</param>
        protected void SetException(Exception err)
        {
            this.SetException(err, false);
        }

        /// <summary>
        /// Sets the LastException property, making HasException true.  Fires the OnException event depending on 
        /// supplied SupressOnExceptionEvent parameter.  Re-throws the exception if ThrowExceptions is true, 
        /// after firing the OnException event.
        /// </summary>
        /// <param name="err">The exception object</param>
        /// <param name="SupressOnExceptionEvent">Fire the OnException event? (true == yes)</param>
        protected void SetException(Exception err, bool SupressOnExceptionEvent)
        {
            this._lastException = err;

            if (!SupressOnExceptionEvent)
            {
                EventHandler handler = this.OnException;
                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }

            if (this._throwExceptions)
            {
                throw err;
            }
        }

        /// <summary>
        /// Is this object currently holding an exception
        /// </summary>
        public bool HasException
        {
            get { return (this._lastException != null); }
        }

        /// <summary>
        /// Returns the last exception passed into SetException
        /// </summary>
        public Exception LastException
        {
            get { return(this._lastException); }
        }

        /// <summary>
        /// If this object has an exception defined it throws it.
        /// </summary>
        public void ThrowLastException()
        {
            if (this.HasException)
                throw this.LastException;
        }

        /// <summary>
        /// Event is fired when SetException is called
        /// </summary>
        public event EventHandler OnException;

        /// <summary>
        /// If true causes an ExceptionHoldingObject to throw and not hold the exception.  The OnException
        /// event will fire before the exception is re-thrown.
        /// </summary>
        public bool ThrowExceptions
        {
            get { return (this._throwExceptions); }
            set { this._throwExceptions = value; }
        }
    }
}
