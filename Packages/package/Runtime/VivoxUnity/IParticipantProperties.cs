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

namespace VivoxUnity
{
    /// <summary>
    /// Common properties that represent a player in a channel.
    /// </summary>
    public interface IParticipantProperties
    {
        /// <summary>
        /// True if this participant corresponds to the currently connected user.
        /// </summary>
        bool IsSelf { get; }
        /// <summary>
        /// If true, the user is in audio.
        /// </summary>
        bool InAudio { get; }
        /// <summary>
        /// If true, the user is in text.
        /// </summary>
        bool InText { get; }
        /// <summary>
        /// If true, the user has no available capture device.
        /// </summary>
        bool UnavailableCaptureDevice { get; }
        /// <summary>
        /// If true, the user has no available render device.
        /// </summary>
        bool UnavailableRenderDevice { get; }
        /// <summary>
        /// If true, the user is speaking.
        /// </summary>
        bool SpeechDetected { get; }
        /// <summary>
        /// The energy or intensity of the participant audio.
        /// </summary>
        /// <remarks>
        /// <para>This determines how loud the user is speaking. This is a value between 0 and 1.</para>
        /// <para>By default, participant property events send only on participant state change (for example, when a participant starts talking, stops talking, is muted, or is unmuted). If set to a per second rate, messages send at that rate if there has been a change since the last update message. This is always true unless the participant is muted through the SDK, which causes no audio energy and no state changes.</para>
        /// <para>Caution: Audio energy might not reach 0 when the user stops speaking, even if the capture device is physically muted. This is because the SDK might still pick up background noise or interference from the physically muted device. To ensure that audio energy reaches 0, the capture device must be muted through the SDK.</para>
        /// </remarks>
        double AudioEnergy { get; }
        /// <summary>
        /// Set the gain for this user. This applies only for the currently signed in user.
        /// </summary>
        /// <remarks>
        /// The valid range is between -50 and 50.
        /// Positive values increase volume, and negative values decrease volume. 
        /// Zero (default) leaves the volume unchanged.
        /// </remarks>
        int LocalVolumeAdjustment { get; set; }
        /// <summary>
        /// Silence or un-silence this participant only for the currently connected user.
        /// </summary>
        bool LocalMute { get; set; }
        /// <summary>
        /// Indicates whether the user has been muted for all users.
        /// </summary>
        bool IsMutedForAll { get; }
    }
}
