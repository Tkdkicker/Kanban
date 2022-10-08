using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace Kanban
{
    public static class EmailExtensions
    {
        /// <summary>
        /// Settings delegate
        /// </summary>
        /// <param name="this">SMTP client</param>
        private delegate void Settings(SmtpClient @this);

        /// <summary>
        /// Get sender email from network credentials
        /// <param name="this">SMTP client</param>
        /// <returns>Sender email address</returns>
        public static string GetSenderEmail(this SmtpClient @this)
        {
            return (@this.Credentials as NetworkCredential)?.UserName;
        }

        /// <summary>
        /// Set defaults for EFFECT Photonics
        /// </summary>
        /// <param name="this">SMTP client</param>
        public static void SetDefaults(this SmtpClient @this)
        {
            SetDefaults(@this, false);
        }

        /// <summary>
        /// Set defaults for EFFECT Photonics
        /// </summary>
        /// <param name="this">SMTP client</param>
        public static void SetDefaults(this SmtpClient @this, bool verify)
        {
            // Define settings order and exception
            Exception exception = null;

            // Define application name
            string name = Assembly.GetEntryAssembly()?.FullName;

            if (string.IsNullOrEmpty(name))
            {
                name = "unknown application";
            }

            // Enumerate settings
            foreach (Settings setting in GetDefaults())
            {
                // Call settings
                setting(@this);

                if (!verify)
                {
                    return;
                }

                // Get user name
                string user = GetSenderEmail(@this);

                // Try to send an email
                try
                {
                    @this.Send(user, user, "Test message", "Test message sent from " + name);
                    return;
                }

                catch (Exception e)
                {
                    // Catch exception
                    exception = e;
                }
            }

            // Throw exception
            throw exception;

        }

        /// <summary>
        /// Get defaults
        /// </summary>
        /// <returns>Enumerable settings</returns>
        private static IEnumerable<Settings> GetDefaults()
        {
            yield return SetDefualtsOutlook;
            yield return SetDefualtsGmail;
        }

        /// <summary>
        /// Set defaults for EFFECT Photonics
        /// </summary>
        /// <param name="this">SMTP client</param>
        public static void SetDefualtsOutlook(this SmtpClient @this)
        {
            @this.Host = "smtp.office365.com";
            @this.Port = 587;
            @this.EnableSsl = true;
            @this.DeliveryMethod = SmtpDeliveryMethod.Network;
            @this.UseDefaultCredentials = false;
            #region @this.Credentials = new NetworkCredential(no-reply@effectphotonics.com, ...)
            #region Confidential
            #region Confidential
            #region Confidential
            @this.Credentials = new NetworkCredential("no-reply@effectphotonics.com", "Te4QUnbP#8");
            #endregion
            #endregion
            #endregion
            #endregion
            @this.Timeout = 5000;
        }

        /// <summary>
        /// Set defaults for EFFECT Photonics
        /// </summary>
        /// <param name="this">SMTP client</param>
        public static void SetDefualtsGmail(this SmtpClient @this)
        {
            @this.Host = "smtp.gmail.com";
            @this.Port = 587;
            @this.EnableSsl = true;
            @this.DeliveryMethod = SmtpDeliveryMethod.Network;
            @this.UseDefaultCredentials = false;
            #region @this.Credentials = new NetworkCredential(no-reply@effectphotonics.nl, ...)
            #region Confidential
            #region Confidential
            #region Confidential
            @this.Credentials = new NetworkCredential("no-reply@effectphotonics.nl", "Te4QUnbP#8");
            #endregion
            #endregion
            #endregion
            #endregion
            @this.Timeout = 5000;
        }
    }
}