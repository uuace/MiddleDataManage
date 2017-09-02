﻿using IntegratedManagement.IRepository.FinancialModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegratedManagement.Entity.FinancialModule.JournalRelationMap;
using IntegratedManagement.Entity.Param;
using IntegratedManagement.RepositoryDapper.BaseRepository;
using IntegratedManagement.Entity.Result;
using System.Data;
using Dapper;
using IntegratedManagement.Core.Document;

namespace IntegratedManagement.RepositoryDapper.FinancialModule
{
    /*===============================================================================================================================
	*	Create by Fancy at 2017/8/29 16:45:54
	===============================================================================================================================*/
    public class JournalRelationMapDapperRepository : IJournalRelationMapRepository
    {
        public async Task<List<JournalRelationMap>> GetJournalRelationMapList(QueryParam Param)
        {
            List<JournalRelationMap> collection = null;
            using (var conn = SqlConnectionFactory.CreateSqlConnection())
            {
                conn.Open();

                string sql = $"SELECT  top {Param.limit} {Param.select} FROM T_JournalRelationMap t0 left JOIN T_JournalRelationMapItem t1 on t0.DocEntry = t1.DocEntry {Param.filter + " " + Param.orderby} ";
                try
                {
                    var coll = await conn.QueryParentChildAsync<JournalRelationMap, JournalRelationMapLine, int>(sql, p => p.DocEntry, p => p.JournalRelationMapLines, splitOn: "DocEntry");
                    collection = coll.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
                return collection;
            }
        }

        public async Task<SaveResult> SaveJournalRelationMap(JournalRelationMap JournalSource)
        {
            SaveResult saveRlt = new SaveResult();
            saveRlt.UniqueKey = JournalSource.TransId.ToString();//回传接收方的主键
            using (IDbConnection conn = SqlConnectionFactory.CreateSqlConnection())
            {
                conn.Open();
                IDbTransaction dbTransaction = conn.BeginTransaction();
                try
                {
                    string insertSql = @"INSERT INTO T_JournalRelationMap
                                            (Number,TransId,BPLId,ERPOrderNum,SourceTable,WorkFlow,RefDate,DueDate,TaxDate,CreateDate)
                                            VALUES
                                            (@Number,@TransId,@BPLId,@ERPOrderNum,@SourceTable,@WorkFlow,@RefDate,@DueDate,@TaxDate,@CreateDate)select SCOPE_IDENTITY();";
                    string insertItemSql = @"INSERT INTO T_JournalRelationMapItem
                                            (DocEntry,LineNum,TransId,LineId,BPLId,AcctCode,ShorName,ExpenseType,PayCode,WhsCode,ERPCardCode,ERPBaseCardCode,ERPDocEntry,ERPBaseNum,CardCode,CardName,Credit,Debit)
                                            VALUES
                                            (@DocEntry,@LineNum,@TransId,@LineId,@BPLId,@AcctCode,@ShorName,@ExpenseType,@PayCode,@WhsCode,@ERPCardCode,@ERPBaseCardCode,@ERPDocEntry,@ERPBaseNum,@CardCode,@CardName,@Credit,@Debit)";

                    object DocEntry = await conn.ExecuteScalarAsync(insertSql,
                        new
                        {
                            Number=JournalSource.Number
                            ,TransId=JournalSource.TransId
                            ,BPLId=JournalSource.BPLId
                            ,ERPOrderNum=JournalSource.ERPOrderNum
                            ,SourceTable=JournalSource.SourceTable
                            ,WorkFlow=JournalSource.WorkFlow
                            ,RefDate=JournalSource.RefDate
                            ,DueDate=JournalSource.DueDate
                            ,TaxDate=JournalSource.TaxDate
                            ,CreateDate=JournalSource.CreateDate
                        }, dbTransaction);
                    saveRlt.ReturnUniqueKey = DocEntry.ToString();//回传保存订单的主键
                    await conn.ExecuteAsync(insertItemSql, DocumentItemHandle<JournalRelationMapLine>.GetDocumentItems(JournalSource.JournalRelationMapLines, Convert.ToInt32(DocEntry)), dbTransaction);

                    dbTransaction.Commit();
                    saveRlt.Code = 0;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    saveRlt.Code = 1;
                    saveRlt.Message = ex.Message;
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
                return saveRlt;

            }
        }
    }
}
