﻿using System;

namespace Secs4Net
{
    public class PrimaryMessageWrapper
    {
        readonly SecsGem.Header _header;
        readonly SecsGem _secsGem;
        public int MessageId => _header.SystemBytes;
        public SecsMessage Message { get; }

        bool _isReplied;

        internal PrimaryMessageWrapper(SecsGem secsGem, SecsGem.Header header, SecsMessage msg)
        {
            _secsGem = secsGem;
            _header = header;
            Message = msg;
        }

        /// <summary>
        /// Each PrimaryMessageWrapper can invoke Reply method once.
        /// Since message replied, method return false.
        /// </summary>
        /// <param name="replyMessage"></param>
        /// <returns>ture, if reply message sent.</returns>
        public bool Reply(SecsMessage replyMessage)
        {
            if (_isReplied)
                return false;

            _isReplied = true;

            if (!_header.ReplyExpected || _secsGem.State != ConnectionState.Selected)
                return true;

            replyMessage = replyMessage ?? new SecsMessage(9, 7, false, "Unknown Message", Item.B(_header.Bytes));
            replyMessage.ReplyExpected = false;

            _secsGem.SendDataMessageAsync(replyMessage, replyMessage.S == 9 ? _secsGem.NewSystemId : _header.SystemBytes);

            return true;
        }

        public override string ToString() => Message.ToString();

    }
}