#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.DataAccessHelper;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.InsertManagerObjects;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using System.Linq;
using System.Windows.Documents;

#endregion

namespace DriveHUD.PlayerXRay.BusinessHelper
{
    public class NotesInsertManager
    {
        #region Delegates

        public delegate void ManagerMessageDelegate(string message, bool inWindow);

        public delegate void ManagerProgressChangedDelegate(int percent);

        public delegate void ManagerStateChangedDelegate(WorkerState state);

        #endregion

        private BackgroundWorker m_worker;

        private NotesInsertManager()
        {
            Notes = new List<NoteObject>();
        }

        public NotesInsertManager(NoteObject note) : this()
        {
            Notes.Add(note);
        }

        public NotesInsertManager(IEnumerable<NoteObject> notes) : this()
        {
            Notes.AddRange(notes);
        }

        public List<NoteObject> Notes { private get; set; }
        public event ManagerStateChangedDelegate ManagerStateChanged;
        public event ManagerMessageDelegate ManagerMessageReceived;
        public event ManagerProgressChangedDelegate ManagerProgressChanged;
        private bool m_paused;

        public void StartWork()
        {
            m_paused = false;
            m_worker = new BackgroundWorker();
            m_worker.DoWork += WorkerDoWork;
            m_worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
            m_worker.ProgressChanged += WorkerProgressChanged;

            m_worker.WorkerReportsProgress = true;
            m_worker.WorkerSupportsCancellation = true;

            if (ManagerStateChanged != null)
                ManagerStateChanged(WorkerState.Working);

            m_worker.RunWorkerAsync();
        }

        public void Stop()
        {
            if (m_paused)
                m_paused = false;
            m_worker.CancelAsync();
        }

        public void Pause()
        {
            m_paused = true;
        }

        public void Resume()
        {
            m_paused = false;
        }

        private void CheckPause()
        {
            if (m_paused)
            {
                if (ManagerStateChanged != null)
                    ManagerStateChanged(WorkerState.Paused);
            }

            while (m_paused)
            {
                System.Threading.Thread.Sleep(100);
            }

            if (ManagerStateChanged != null)
                ManagerStateChanged(WorkerState.Working);
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != -1)
                if (ManagerProgressChanged != null)
                    ManagerProgressChanged(e.ProgressPercentage);
            if (e.UserState != null)
                if (ManagerMessageReceived != null)
                    ManagerMessageReceived(e.UserState.ToString(), false);
        }

        private void SendMsg(string msg, bool inWindow)
        {
            if (ManagerMessageReceived != null)
                ManagerMessageReceived(msg, inWindow);
        }

        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ManagerStateChanged != null)
                ManagerStateChanged(WorkerState.Idle);

            if (e.Cancelled)
            {
                goto Idle;
            }

            SendMsg(
                (int) e.Result == 0
                    ? "Operation completed succesfully"
                    : $"Operation failed. {(int) e.Result} note messages were not added to database",
                true);

            Idle:
            ManagerProgressChanged?.Invoke(0);

            SendMsg("Idle", false);
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            m_worker.ReportProgress(0, "Process started");               

            List<ProcessNoteObject> processNotes = new List<ProcessNoteObject>();
            List<DatabaseNote> databaseNotes = new List<DatabaseNote>();

            foreach (NoteObject note in Notes)
            {
                if (m_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                CheckPause();
                m_worker.ReportProgress((int)Math.Round(Notes.IndexOf(note) / (double)Notes.Count * 25),
                                        "Processing " + note.Name);

                //m_worker.ReportProgress(28, "Creating list of players");
                //m_worker.ReportProgress(-1, "Retrieving existing notes");
                //m_worker.ReportProgress((int)Math.Round((processNotes.IndexOf(note) / (double)processNotes.Count * 50 + 30)),
                //                        "Generating note message for " + note.Note.Name);
                //m_worker.ReportProgress(30);
                databaseNotes.Add(NoteManager.GetPlayerNote(note));   
            }


//foreach (NoteObject note in Notes)
//            {
//                if (m_worker.CancellationPending)
//                {
//                    e.Cancel = true;
//                    return;
//                }
//                CheckPause();
//                m_worker.ReportProgress((int) Math.Round((Notes.IndexOf(note)/(double) Notes.Count*25)),
//                                        "Processing " + note.Name);

//                if (note.Settings.Cash)
//                {
//                    if (client == ClientType.HoldemManager)
//                    {
//                        ProcessNoteObject pn = GetProcessHmNote(note, true, processNotes);
//                        GC.Collect();
//                        if (pn != null)
//                        {
//                            processNotes.Add(pn);
//                        }
//                    }
//                    else
//                    {
//                        ProcessNoteObject pn = GetProcessPtNote(note, true, processNotes);
//                        GC.Collect();
//                        if (pn != null)
//                        {
//                            processNotes.Add(pn);
//                        }
//                    }
//                }

//                if (!note.Settings.Tournament) continue;
//                if (client == ClientType.HoldemManager)
//                {
//                    ProcessNoteObject pn = GetProcessHmNote(note, false, processNotes);
//                    GC.Collect();
//                    if (pn != null)
//                    {
//                        processNotes.Add(pn);
//                    }
//                }
//                else
//                {
//                    ProcessNoteObject pn = GetProcessPtNote(note, false, processNotes);
//                    GC.Collect();
//                    if (pn != null)
//                    {
//                        processNotes.Add(pn);
//                    }
//                }
//            }            

            List<long> allPlayersIDsList = new List<long>();

            m_worker.ReportProgress(28, "Creating list of players");


            if (m_worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            CheckPause();

            foreach (ProcessNoteObject note in processNotes)
            {
                allPlayersIDsList.AddRange(note.PlayerIDs);
            }
            IEnumerable<long> iPlayers = allPlayersIDsList.Distinct();

            CheckPause();
            if (m_worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            m_worker.ReportProgress(-1, "Retrieving existing notes");

            //databaseNotes = DAL.GetPlayersNotes(iPlayers);

            m_worker.ReportProgress(30);

            //foreach (ProcessNoteObject note in processNotes)
            //{
            //    GC.Collect();
            //    if (m_worker.CancellationPending)
            //    {
            //        e.Cancel = true;
            //        return;
            //    }
            //    m_worker.ReportProgress((int)Math.Round((processNotes.IndexOf(note) / (double)processNotes.Count * 50 + 30)),
            //                            "Generating note message for " + note.Note.Name);

            //    IEnumerable<long> pl = note.PlayerIDs.Distinct();
                
            //    foreach (long player in pl)
            //    {
            //        DatabaseNote dbNote = databaseNotes.FirstOrDefault(p => p.PlayerID == player);

            //        dbNote = GetDbNote(note,dbNote, player);

            //        if (dbNote.Added && !databaseNotes.Contains(dbNote))
            //            databaseNotes.Add(dbNote);
            //    }
            //}

            int fails = 0;

            //foreach (DatabaseNote note in databaseNotes)
            //{
            //    if (m_worker.CancellationPending)
            //    {
            //        e.Cancel = true;
            //        return;
            //    }
            //    CheckPause();

            //    if (!note.Added && !note.Modified)
            //        continue;

            //    m_worker.ReportProgress((int)Math.Round((databaseNotes.IndexOf(note) / (double)databaseNotes.Count * 20 + 80)),
            //                            "Adding note to database " + databaseNotes.IndexOf(note) + " / " + databaseNotes.Count);

            //    string query = string.Empty;

            //    if (note.Added)
            //    {
            //        query += string.Format(
            //            "Insert into HEMPlayerNotes (player_id,note,icon_id) Values ({0},'{1}',4);\n", note.PlayerID,
            //            note.Message.Replace("'", "''"));
            //    }
            //    else
            //    {
            //        query += string.Format("Update HEMPlayerNotes SET note='{0}' WHERE player_id={1};\n", note.Message.Replace("'", "''"),
            //                               note.PlayerID);
            //    }

            //    PostgresResult dbResult = PostgresQueryHelper.ExecuteNonQuery(query,
            //                                                              NotesAppSettingsHelper.ConnectionString);

            //    if (dbResult.Success) continue;
            //    fails++;
            //    Log.Error(dbResult.Exception);
            //}

            CheckPause();
            if (m_worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            
            m_worker.ReportProgress(100);

            e.Result = fails;
        }

        private static List<string> GetHoleCards(ProcessNoteObject pn, long playerID)
        {
            int numberOfHoleCards = NotesAppSettingsHelper.CurrentNotesAppSettings.HoleCardsNumber;
            bool allHoleCards = NotesAppSettingsHelper.CurrentNotesAppSettings.AllHoleCards;

            List<long> playerCards = new List<long>();
            List<ProcessPlayerObject> players = pn.Players.Where(p => p.ID == playerID).ToList();

            foreach (ProcessPlayerObject player in players)
            {
                if (player.HandID == 0)
                    continue;
                playerCards.Add(player.HandID);
            }

            Dictionary<int, int> cardOccurences = new Dictionary<int, int>();

            foreach (int card in playerCards)
            {
                if (cardOccurences.ContainsKey(card))
                    continue;
                cardOccurences.Add(card, playerCards.Where(p => p == card).Count());
            }
            var sortedCardOccurences = (from entry in cardOccurences orderby entry.Value descending ,entry.Key ascending select entry);

            List<string> result = new List<string>();

            if (allHoleCards)
            {
                int rows = (int)decimal.Ceiling((decimal)(sortedCardOccurences.Count() / 11.00));
                for (int i = 0; i < rows; i++)
                {
                    result.Add(GetHoleCardsRow(11, sortedCardOccurences, i * 11));
                }
            }
            else
            {
                if (numberOfHoleCards <= 11)
                {
                    result.Add(GetHoleCardsRow(numberOfHoleCards, sortedCardOccurences, 0));
                }
                else
                {
                    int rows = (int)decimal.Ceiling((decimal)(sortedCardOccurences.Count() / 11.00));
                    for (int i = 0; i < rows; i++)
                    {
                        result.Add(GetHoleCardsRow(11, sortedCardOccurences, i * 11));
                    }
                }
            }

            return result;
        }

        private static string GetHoleCardsRow(int count, IEnumerable<KeyValuePair<int, int>> cards, int startingIndex)
        {
            List<int> cardsList = new List<int>();

            for (int i=startingIndex; i<startingIndex + count; i++)
            {
                if (i >= cards.Count())
                    break;

                cardsList.Add(cards.ElementAt(i).Key);
            }

            string result = "{";

            foreach (int card in cardsList)
            {
                result += HoleCardsHelper.GetHoleCard(card) + ",";
            }

            if (result.EndsWith(","))
                result = result.Remove(result.LastIndexOf(","), 1);

            result += "}";
            return result;
        }

        private static void AppendMessageToNote(ProcessNoteObject pn, string message, DatabaseNote note)
        {
            List<string> lines = note.Message != null ? note.Message.Split('\n').ToList() : new List<string>();
            NoteStageType stage = GetNoteStage(pn.Note);

            int headerPosition = GetNoteStageHeaderPosition(lines, stage, true);
            if (lines.FirstOrDefault(p => p.Contains(pn.Note.DisplayedNote)) == null)
                lines.Insert(headerPosition+1, message);
            else
            {
                int existingNotePosition = lines.IndexOf(lines.First(p => p.Contains(pn.Note.DisplayedNote)));
                lines.RemoveAt(existingNotePosition);
                while (existingNotePosition <lines.Count && lines[existingNotePosition].StartsWith("{"))
                {
                    lines.RemoveAt(existingNotePosition);
                }
                lines.Insert(existingNotePosition, message);
            }
            note.Message = ConvertStringArrayToString(lines);
        }

        private static int GetNoteStageHeaderPosition(List<string> lines, NoteStageType stage, bool createHeader)
        {
            string line = string.Empty;

            switch (stage)
            {
                case NoteStageType.PreFlop:
                    line = lines.Find(p => p.Contains("| PRE-FLOP |"));
                    break;
                case NoteStageType.Flop:
                    line = lines.Find(p => p.Contains("|   FLOP   |"));
                    break;
                case NoteStageType.Turn:
                    line = lines.Find(p => p.Contains("|   TURN   |"));
                    break;
                case NoteStageType.River:
                    line = lines.Find(p => p.Contains("|   RIVER  |"));
                    break;
            }

            if (!string.IsNullOrEmpty(line))
                return lines.IndexOf(line);

            if (!createHeader)
                return -1;

            string header = string.Empty;

            switch (stage)
            {
                case NoteStageType.PreFlop:
                    header = "-----------------===| PRE-FLOP |===-----------------";
                    break;
                case NoteStageType.Flop:
                    header = "-------------------===|   FLOP   |===------------------";
                    break;
                case NoteStageType.Turn:
                    header = "-------------------===|   TURN   |===------------------";
                    break;
                case NoteStageType.River:
                    header = "-------------------===|   RIVER  |===------------------";
                    break;
            }

            switch (stage)
            {
                case NoteStageType.PreFlop:
                    int forwardHeader = GetNoteStageHeaderPosition(lines, NoteStageType.Flop, false);
                    if (forwardHeader == -1)
                    {
                        forwardHeader = GetNoteStageHeaderPosition(lines, NoteStageType.Turn, false);

                        if (forwardHeader == -1)
                        {
                            forwardHeader = GetNoteStageHeaderPosition(lines, NoteStageType.River, false);
                        }
                    }
                    if (forwardHeader == -1)
                        lines.Add(header);
                    else
                        lines.Insert(forwardHeader != 0 ? forwardHeader - 1 : forwardHeader, header);
                    return GetNoteStageHeaderPosition(lines, NoteStageType.PreFlop, false);
                case NoteStageType.Flop:
                    forwardHeader = GetNoteStageHeaderPosition(lines, NoteStageType.Turn, false);
                    if (forwardHeader == -1)
                    {
                        forwardHeader = GetNoteStageHeaderPosition(lines, NoteStageType.River, false);
                    }
                    if (forwardHeader == -1)
                        lines.Add(header);
                    else
                        lines.Insert(forwardHeader != 0 ? forwardHeader - 1 : forwardHeader, header);
                    return GetNoteStageHeaderPosition(lines, NoteStageType.Flop, false);
                case NoteStageType.Turn:
                     forwardHeader = GetNoteStageHeaderPosition(lines, NoteStageType.River, false);
                    if (forwardHeader == -1)
                        lines.Add(header);
                    else
                        lines.Insert(forwardHeader != 0 ? forwardHeader - 1 : forwardHeader, header);
                    return GetNoteStageHeaderPosition(lines, NoteStageType.Turn, false);
                case NoteStageType.River:
                    lines.Add(header);
                    return GetNoteStageHeaderPosition(lines, NoteStageType.River, false);
            }

            return -1;
        }

        private static NoteStageType GetNoteStage(NoteObject note)
        {
            foreach (StageObject stage in NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList)
            {
                foreach (NoteObject n in stage.Notes)
                {
                    if (n.Name == note.Name)
                        return stage.StageType;
                }

                foreach (InnerGroupObject inner in stage.InnerGroups)
                {
                    foreach (NoteObject n in inner.Notes)
                    {
                        if (n.Name == note.Name)
                            return stage.StageType;
                    }
                }
            }

            return NoteStageType.PreFlop;
        }

       
        private static DatabaseNote GetDbNote(ProcessNoteObject pn, DatabaseNote dbNote, long playerID)
        {
            string message = pn.Note.DisplayedNote;
            if (!pn.Cash)
                message = "[T] " + message;

            int val = pn.Players.Where(p => p.ID == playerID).Count();

            message += ": " + val;

            if (pn.ComparisonPlayers != null)
            {
                int compVal = pn.ComparisonPlayers.Where(p => p.ID == playerID).Count();

                if (compVal > 0)
                {
                    int perc = (int) (((double) val/(double) compVal)*100);
                    if (perc > 0)
                        message += string.Format("/{0} [{1}%]", compVal, perc);
                    else
                    {
                        double percDbl = (((double) val/(double) compVal)*100);
                        message += string.Format("/{0} [{1}%]", compVal, Math.Round(percDbl, 1));
                    }
                }
            }

            if (dbNote != null && dbNote.Message.Contains(message))
                return dbNote;

            if (NotesAppSettingsHelper.CurrentNotesAppSettings.ShowHoleCards)
            {
                message += " ";
                List<string> holeCards = GetHoleCards(pn, playerID);

                if (holeCards.Count == 1)
                {
                    if (holeCards[0].Length > 13)
                        message += "\r\n" + holeCards[0];
                    else
                        message += holeCards[0];
                }
                else
                {
                    foreach (string row in holeCards)
                    {
                        message += "\r\n" + row;
                    }
                }
            }

            if (dbNote == null)
            {
                dbNote = new DatabaseNote { Added = true, PlayerID = playerID };
            }
            else
            {
                dbNote.Modified = true;
            }

            AppendMessageToNote(pn, message, dbNote); 
            return dbNote;
        }

        private static string ConvertStringArrayToString(IList<string> array)
        {
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < array.Count; index++)
            {
                string value = array[index];
                value = value.Trim('\n');
                value = value.Trim('\r');

                builder.Append(value);
                if (index < array.Count - 1)
                    builder.Append("\r\n");
            }
            return builder.ToString();
        }
     
        private static List<ProcessPlayerObject> GetExistingComparison(NoteSettingsObject settings, IEnumerable<ProcessNoteObject> existingNotes, bool cash)
        {
            foreach (ProcessNoteObject note in existingNotes)
            {
                if (note.Cash != cash)
                    continue;

                if (settings.SelectedFiltersComparison.Count > 0)
                {
                    if (note.Note.Settings.SelectedFiltersComparison.Count != settings.SelectedFiltersComparison.Count)
                        return null;

                    foreach (FilterObject filter in settings.SelectedFiltersComparison)
                    {
                        if (note.Note.Settings.SelectedFiltersComparison.Find(p => p.Tag == filter.Tag) == null)
                            return null;
                    }

                    return note.ComparisonPlayers;
                }
                else
                {
                    if (settings.ExcludedCards.Count() > 0)
                    {
                        if (note.Note.Settings.SelectedFiltersComparison.Count != 0)
                            continue;

                        if (!CompareHelpers.CompareStringLists(note.Note.Settings.ExcludedCardsList,
                            settings.ExcludedCardsList))
                            continue;

                        return note.ComparisonPlayers;
                    }
                }
            }

            return null;
        }

        private void ProcessPtNote(NoteObject note)
        {
        }

    }

    public enum WorkerState
    {
        Idle,
        Paused,
        Working
    }
}