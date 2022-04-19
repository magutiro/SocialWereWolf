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
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VivoxUnity
{
    /// <summary>The arguments for ITTSMessageQueue event notifications.</summary>
    public sealed class ITTSMessageQueueEventArgs : EventArgs
    {
        public ITTSMessageQueueEventArgs(TTSMessage message) { Message = message; }

        /// <summary>The text-to-speech (TTS) message.</summary>
        public TTSMessage Message { get; }
    }

    public interface ITTSMessageQueue : IEnumerable<TTSMessage>
    {
        /// <summary>Raised when a TTSMessage is added to the text-to-speech subsystem.</summary>
        event EventHandler<ITTSMessageQueueEventArgs> AfterMessageAdded;

        /// <summary>Raised when a TTSMessage is removed from the text-to-speech subsystem.</summary>
        /// <remarks>This can result from either cancellation or playback completion.</remarks>
        event EventHandler<ITTSMessageQueueEventArgs> BeforeMessageRemoved;

        /// <summary>Raised when playback begins for a TTSMessage in the collection.</summary>
        event EventHandler<ITTSMessageQueueEventArgs> AfterMessageUpdated;

        /// <summary>
        /// Remove all objects from the collection and cancel them.
        /// </summary>
        /// <seealso cref="ITextToSpeech.CancelAll" />
        void Clear();

        /// <summary>
        /// Determine whether a TTSMessage is in the collection.
        /// </summary>
        /// <param name="message">The TTSMessage to locate in the collection.</param>
        bool Contains(TTSMessage message);

        /// <summary>
        /// Get the number of elements contained in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Remove and return the oldest TTSMessage in the collection. This cancels the message.
        /// </summary>
        /// <seealso cref="ITextToSpeech.CancelMessage" />
        TTSMessage Dequeue();

        /// <summary>
        /// Add a message and speak it as the user that the collection belongs to.
        /// </summary>
        /// <param name="message">The TTSMessage to add and speak.</param>
        /// <seealso cref="ITextToSpeech.Speak" />
        void Enqueue(TTSMessage message);

        /// <summary>
        /// Return the oldest TTSMessage in the collection without removing it.
        /// </summary>
        TTSMessage Peek();

        /// <summary>
        /// Remove a specific message from the collection. This cancels the message.
        /// </summary>
        /// <param name="message">The TTSMessage to remove and cancel.</param>
        /// <seealso cref="ITextToSpeech.CancelMessage" />
        bool Remove(TTSMessage message);
    }

    /// <summary>
    /// An interface for events and methods related to text-to-speech.
    /// </summary>
    public interface ITextToSpeech : INotifyPropertyChanged
    {
        /// <summary>
        /// All voices available to the text-to-speech subsystem for speech synthesis.
        /// </summary>
        ReadOnlyCollection<ITTSVoice> AvailableVoices { get; }

        /// <summary>
        /// The voice used by text-to-speech methods called from this ILoginSession.
        /// </summary>
        /// <remarks>
        /// If this is not set, then the SDK default voice is used. You can obtain valid ITTSVoices from AvailableVoices.
        /// When setting this, if the new voice is not available (for example, when loaded from saved settings after updating),
        /// then ObjectNotFoundException is raised.
        /// </remarks>
        ITTSVoice CurrentVoice { get; set; }

        /// <summary>
        /// Inject a new text-to-speech (TTS) message into the TTS subsystem.
        /// </summary>
        /// <param name="message">A TTSMessage that contains the text to be converted into speech and the destination for TTS injection.</param>
        /// <remarks>
        /// The Voice and State properties of the message are set by this function.
        /// For information on how the ITTSVoice that is used for speech synthesis is selected, see CurrentVoice.
        /// Synthesized speech sent to remote destinations plays in connected channel sessions
        /// according to the transmission policy (the same sessions that basic voice transmits to).
        /// </remarks>
        void Speak(TTSMessage message);

        /// <summary>
        /// Cancel a single currently playing or enqueued text-to-speech message.
        /// </summary>
        /// <param name="message">The TTSMessage to cancel.</param>
        /// <remarks>
        /// In destinations with queues, canceling an ongoing message automatically triggers the playback of
        /// the next message. Canceling an enqueued message shifts all later messages up one place in the queue.
        /// </remarks>
        void CancelMessage(TTSMessage message);

        /// <summary>
        /// Cancel all text-to-speech messages in a destination (ongoing and enqueued).
        /// </summary>
        /// <param name="destination">The TTSDestination to clear of messages.</param>
        /// <remarks>
        /// The TTSDestinations QueuedRemoteTransmission and QueuedRemoteTransmissionWithLocalPlayback
        /// share a queue, but are not the same destination. Canceling all messages in one of these destinations
        /// automatically triggers the playback of the next message from the other in the shared queue.
        /// </remarks>
        void CancelDestination(TTSDestination destination);

        /// <summary>
        /// Cancel all text-to-speech messages (ongoing and enqueued) from all destinations.
        /// </summary>
        void CancelAll();

        /// <summary>
        /// Contains all text-to-speech (TTS) messages playing or waiting to be played in all destinations.
        /// </summary>
        /// <remarks>
        /// Use the ITTSMessageQueue events to get notifications of when messages are spoken or canceled, or when playback starts or ends.
        /// Methods to Enqueue(), Dequeue(), Remove(), or Clear() items directly from this collection result in the same
        /// behavior as using other class methods to Speak() or Cancel*() TTS messages in the text-to-speech subsystem.
        /// </remarks>
        ITTSMessageQueue Messages { get; }

        /// <summary>
        /// Retrieve ongoing or enqueued TTSMessages in the specified destination.
        /// </summary>
        /// <param name="destination">The TTSDestination to retrieve messages for.</param>
        /// <returns>A queue containing the messages for a single destination.</returns>
        /// <remarks>
        /// Queued destinations return their ITTSMessageQueue in queue order, and others in the order that speech was injected.
        /// </remarks>
        ReadOnlyCollection<TTSMessage> GetMessagesFromDestination(TTSDestination destination);
    }
}
