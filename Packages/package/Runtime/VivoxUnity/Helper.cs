﻿/*
Copyright (c) 2014-2018 by Mercer Road Corp

Permission to use, copy, modify or distribute this software in binary or source form
for any purpose is allowed only under explicit prior consent in writing from Mercer Road Corp

THE SOFTWARE IS PROVIDED "AS IS" AND MERCER ROAD CORP DISCLAIMS
ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL MERCER ROAD CORP
BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL
DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR
PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS
ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS
SOFTWARE.
*/

using System;

namespace VivoxUnity
{
    public class Helper
    {
        public static ulong serialNumber = 0;
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static vx_message_base_t NextMessage()
        {
            vx_message_base_t msg = VivoxCoreInstance.vx_get_message();
            return msg;
        }

        private static void CheckInitialized()
        {
            if (!VxClient.Instance.Started)
            {
                throw new NotSupportedException("Method can not be called before Vivox SDK is initialized.");
            }
        }
        public static string GetRandomUserId(string prefix)
        {
            CheckInitialized();
            return VivoxCoreInstance.vx_get_random_user_id(prefix);
        }
        public static string GetRandomUserIdEx(string prefix, string issuer)
        {
            CheckInitialized();
            return VivoxCoreInstance.vx_get_random_user_id_ex(prefix, issuer);
        }
        public static string GetRandomChannelUri(string prefix, string realm)
        {
            CheckInitialized();
            return VivoxCoreInstance.vx_get_random_channel_uri(prefix, realm);
        }

        /// <summary>
        /// Obtain the time in seconds from the Unix epoch to now with the option of an added duration.
        /// </summary>
        /// <param name="duration">The timespan ahead of (DateTime.UtcNow - Unix epoch) that you want to have a timestamp for.</param>
        /// <returns>The time in seconds from the Unix epoch (January 1st, 1970, 00:00:00) to DateTime.UtcNow with an added duration.</returns>
        private static int SecondsSinceUnixEpochPlusDuration(TimeSpan? duration = null)
        {
            TimeSpan timestamp = DateTime.UtcNow.Subtract(unixEpoch);
            if (duration.HasValue)
            {
                timestamp = timestamp.Add(duration.Value);
            }

            return (int)timestamp.TotalSeconds;
        }
    }
}
