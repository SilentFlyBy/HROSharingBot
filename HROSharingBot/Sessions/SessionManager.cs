using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HROSharingBot.Sessions
{
    public static class SessionManager
    {
        private static List<Session> sessions = new List<Session>();


        public static T CreateSession<T>(long chatId) where T : Session, new()
        {
            T session;
            TryCreateSession(chatId, out session);

            return session;
        }

        public static bool TryCreateSession<T>(long chatId, out T session) where T : Session, new()
        {
            if (SessionExists(chatId))
            {
                session = null;
                return false;
            }

            var returnSession = new T();
            returnSession.ChatId = chatId;
            sessions.Add(returnSession);

            session = returnSession;
            return true;
        }

        public static void DestroySession(Session session)
        {
            DestroySession(session.ChatId);
        }

        public static void DestroySession(long chatId)
        {
            var session = sessions.Where(s => s.ChatId == chatId).FirstOrDefault();

            if (session != null)
                sessions.Remove(session);
        }

        public static bool SessionExists(long chatId)
        {
            return sessions.Any(s => s.ChatId == chatId);
        }

        public static Session GetSession(long chatId)
        {
            return sessions.Where(s => s.ChatId == chatId).FirstOrDefault();
        }
    }
}
