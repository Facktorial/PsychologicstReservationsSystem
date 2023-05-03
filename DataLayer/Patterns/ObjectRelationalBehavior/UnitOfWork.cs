using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;


namespace DataLayer
{
    ///
    /// Change when changing database
    ///
    using SQLConnection = SqliteConnection;
    using SQLReader = SqliteDataReader;
    using SQLCommand = SqliteCommand;
    using SQLTransaction = SqliteTransaction;

    public interface ICommit
    {
        public bool Commit();
        public bool Rollback();
    }

    public class TransactionWrapper
    {
        public TransactionWrapper() { }
        public SQLCommand Command;
        public SQLTransaction Transaction;
        public List<int> Items;
    }
    public class UnitOfWork<T, InnerType> : ICommit
        where T : IEnumerable<InnerType>
        where InnerType : IEntity, new ()
    {
        private readonly T _domainObject;
        private readonly SqlConnector _sqlConnect;
        private TransactionWrapper _transaction;

        public UnitOfWork() { }

        public UnitOfWork(T obj, SqlConnector sqlConnect)
        {
            _domainObject = obj;
            _sqlConnect = sqlConnect;
        }

        public bool Run(StateOfCollection state)
        {
            var container = _domainObject.ToList();

            var ls = new List<(string, List<int>)>
            {
                DeleteItems(container, state.Removed),
                UpdateItems(container, state.Changed),
                InsertItems(container, state.Added)
            };

            using (SQLConnection connection = new SQLConnection(_sqlConnect.ConnectionString))
            {
                connection.Open();

                _transaction = new TransactionWrapper();

                var commands = new List<SQLCommand>
                {
                    new SQLCommand(ls[0].Item1, connection),
                    new SQLCommand(ls[1].Item1, connection),
                    new SQLCommand(ls[2].Item1, connection),
                };

                for (int i = 0; i < ls.Count(); i++)
                {
                    _transaction.Command = commands[i];
                    _transaction.Items = ls[i].Item2;
                    using (SQLTransaction transaction = connection.BeginTransaction())
                    {
                        _transaction.Transaction = transaction;
                        if (!Commit()) { return false; }
                    }
                }
            }

            return true;
        }

        public bool Commit()
        {
            try
            {
                var command = _transaction.Command;

                foreach (var id in _transaction.Items)
                {
                    InnerType? tmp = default(InnerType);
                    if (!command.CommandText.Contains("DELETE"))
                    {
                        tmp = _domainObject.Where(x => x.Id == id).First();
                        // FIXME
                        if (tmp == null) { return false; }
                        SQLHelper<InnerType>.MapParameters(command, tmp, id);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Id", id);
                    }

                    Console.WriteLine("command: " + command.CommandText);

                    command.Transaction = _transaction.Transaction;
                    command.ExecuteNonQuery();

                    command.Parameters.Clear();
                }

                _transaction.Transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Rollback();
            }

            return true;
        }

        public bool  Rollback()
        {
            _transaction.Transaction.Rollback();
            return false;
        }

        private (string, List<int>) InsertItems(List<InnerType> items, HashSet<int> ls)
        {
            var to_insert = items
                .Where(x => ls.Contains(x.Id))
                //.Select(x => (InnerType)Convert.ChangeType(x, typeof(InnerType)))
                .Select(x => x.Id)
                .ToList();

            string sql = $"INSERT INTO {SQLHelper<InnerType>.TableName()} ({SQLHelper<InnerType>.GetColumnNames()}) VALUES ({SQLHelper<InnerType>.GetParameterNames()});";

            return (sql, to_insert.ToList());
        }

        private (string, List<int>) UpdateItems(List<InnerType> items, HashSet<int> ls)
        {
            var to_update = items
                .Where(x => ls.Contains(x.Id))
                //.Select(x => (InnerType)Convert.ChangeType(x, typeof(InnerType)))
                .Select(x => x.Id)
                .ToList();

            string sql = $"UPDATE {SQLHelper<InnerType>.TableName()} SET {SQLHelper<InnerType>.GetSetExpressions()} WHERE Id = @Id;";

            return (sql, to_update.ToList());
        }

        private (string, List<int>) DeleteItems(List<InnerType> items, HashSet<int> ls)
        {
            string sql = $"DELETE FROM {SQLHelper<InnerType>.TableName()} WHERE Id = @Id;";

            return (sql, ls.ToList());
        }
    }
}
