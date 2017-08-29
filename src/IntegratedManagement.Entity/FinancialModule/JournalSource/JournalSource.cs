﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedManagement.Entity.FinancialModule.JournalSource
{
    /*===============================================================================================================================
	*	Create by Fancy at 2017/8/29 9:15:53
	===============================================================================================================================*/
    public class JournalSource
    {
        public JournalSource()
        {
            this.JournalSourceLines = new List<JournalSourceLine>();
        }
        /// <summary>
        /// 编号
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public int TransId { get; set; }

        public int TransType { get; set; }

        /// <summary>
        /// 分支编号
        /// </summary>
        public int BPLId { get; set; }

        /// <summary>
        /// ERP订单号
        /// </summary>
        public int ERPOrderNum { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public string SourceTable { get; set; }

        /// <summary>
        /// 业务流
        /// </summary>
        public string WorkFlow { get; set; }

        /// <summary>
        /// 过账日期
        /// </summary>
        public DateTime RefDate { get; set; }
        /// <summary>
        /// 到期日
        /// </summary>
        public DateTime DueDate { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        public DateTime TaxDate { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        public List<JournalSourceLine> JournalSourceLines { get; set; }
    }
}
