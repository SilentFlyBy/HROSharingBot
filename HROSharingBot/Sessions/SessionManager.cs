using System.Collections.Generic;
using System.Linq;

namespace HROSharingBot.Sessions
{
    public static class SessionManager
    {
        private static readonly List<Session> Sessions = new List<Session>();


        public static T CreateSession<T>(long chatId) where T : Session, new()
        {
            return TryCreateSession(chatId, out T session) == false ? null : session;
        }

        private static bool TryCreateSession<T>(long chatId, out T session) where T : Session, new()
        {
            if (SessionExists(chatId))
            {
                session = null;
                return false;
            }

            var returnSession = new T {ChatId = chatId};
            Sessions.Add(returnSession);

            session = returnSession;
            return true;
        }

        public static void DestroySession(Session session)
        {
            if (session != null)
                Sessions.Remove(session);
        }

        private static void DestroySession(long chatId)
        {
            var session = Sessions.FirstOrDefault(s => s.ChatId == chatId);

            DestroySession(session);
        }

        public static bool SessionExists(long chatId)
        {
            return Sessions.Any(s => s.ChatId == chatId);
        }

        public static Session GetSession(long chatId)
        {
            return Sessions.FirstOrDefault(s => s.ChatId == chatId);
        }
    }
}