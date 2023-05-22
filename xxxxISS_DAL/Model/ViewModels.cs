using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RES_Repo.Globalization;

namespace ISS_DAL.ViewModels
{
    public class StockALLVM
    {
        public Nullable<Guid> ItemGUID { get; set; }
        public int? TotalItem { get; set; }
    }
    public class StockItemDistributionDataTableModel
    {
        [Display(Name = "ItemStockBalanceGUID", ResourceType = typeof(resxDbFields))]

        public Guid ItemStockBalanceGUID { get; set; }
        [Display(Name = "TrackStockUploadGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> TrackStockUploadGUID { get; set; }
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public string ItemGUID { get; set; }
        [Display(Name = "StockGUID", ResourceType = typeof(resxDbFields))]
        public string StockGUID { get; set; }
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public string ItemName { get; set; }
        [Display(Name = "StockName", ResourceType = typeof(resxDbFields))]
        public string StockName { get; set; }

        [Display(Name = "TotalItem", ResourceType = typeof(resxDbFields))]
        public int? TotalItem { get; set; }

        public int? OrderId { get; set; }

        public bool Active { get; set; }
        public byte[] dataItemStockBalanceRowVersion { get; set; }


    }

    public class StockItemOverviewDataTableModel
    {

        [Display(Name = "ItemStockEmergencyReserveGUID", ResourceType = typeof(resxDbFields))]

        public Guid ItemStockEmergencyReserveGUID { get; set; }
        [Display(Name = "TrackStockUploadGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> TrackStockUploadGUID { get; set; }
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public string ItemGUID { get; set; }
        [Display(Name = "QuantityToBeReserved", ResourceType = typeof(resxDbFields))]
        public int? QuantityToBeReserved { get; set; }
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public string ItemName { get; set; }
        [Display(Name = "ForWhere", ResourceType = typeof(resxDbFields))]
        public string ForWhere { get; set; }

        [Display(Name = "ExpectedDateToDispatch", ResourceType = typeof(resxDbFields))]
        public DateTime? ExpectedDateToDispatch { get; set; }


        [Display(Name = "TotalItemInAllStock", ResourceType = typeof(resxDbFields))]
        public int? TotalItemInAllStock { get; set; }

        [Display(Name = "PipelineOrdersPlaced", ResourceType = typeof(resxDbFields))]
        public int? PipelineOrdersPlaced { get; set; }
        [Display(Name = "TotalStockWithPipeline", ResourceType = typeof(resxDbFields))]
        public int? TotalStockWithPipeline { get; set; }
        [Display(Name = "TotalReservedforEmergency", ResourceType = typeof(resxDbFields))]
        public int? TotalReservedforEmergency { get; set; }
        public int? OrderId { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemStockEmergencyReserveRowVersion { get; set; }


    }

    public class ItemPipelineDataTableModel
    {
        [Display(Name = "ItemPipelineGUID", ResourceType = typeof(resxDbFields))]

        public Guid ItemPipelineGUID { get; set; }
        [Display(Name = "ItemPipelineUploadGUID", ResourceType = typeof(resxDbFields))]
        public Nullable<Guid> ItemPipelineUploadGUID { get; set; }
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public string ItemGUID { get; set; }

        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public string ItemName { get; set; }

        [Display(Name = "StockGUID", ResourceType = typeof(resxDbFields))]
        public string StockGUID { get; set; }

        [Display(Name = "StockName", ResourceType = typeof(resxDbFields))]
        public string StockName { get; set; }

        public int? OrderId { get; set; }

        [Display(Name = "TotalItem", ResourceType = typeof(resxDbFields))]
        public int? Quantity { get; set; }

        [Display(Name = "ETA", ResourceType = typeof(resxDbFields))]
        public DateTime? ETA { get; set; }

        public bool Active { get; set; }
        public byte[] dataItemPipelineRowVersion { get; set; }


    }
    public class ItemOverviewPipelineDataTableModel
    {
        [Display(Name = "ItemOverviewGUID", ResourceType = typeof(resxDbFields))]

        public Guid ItemOverviewGUID { get; set; }

        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public string ItemGUID { get; set; }

        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public string ItemName { get; set; }


        [Display(Name = "TotalItemAllStocks", ResourceType = typeof(resxDbFields))]
        public int? TotalItemAllStocks { get; set; }
        [Display(Name = "TotalItemPipeLine", ResourceType = typeof(resxDbFields))]
        public int? TotalItemPipeLine { get; set; }
        [Display(Name = "TotalItemStockWithPipeLine", ResourceType = typeof(resxDbFields))]
        public int? TotalItemStockWithPipeLine { get; set; }
        [Display(Name = "TotalReservedForEmergency", ResourceType = typeof(resxDbFields))]
        public int? TotalReservedForEmergency { get; set; }


        public int? OrderId { get; set; }
        public bool Active { get; set; }
        public byte[] dataItemOverviewRowVersion { get; set; }


    }

    public class ItemDataTableModel
    {
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public System.Guid ItemGUID { get; set; }
        [Display(Name = "SequencId", ResourceType = typeof(resxDbFields))]
        public int? SequencId { get; set; }
        public byte[] codeISSItemRowVersion { get; set; }
        [Display(Name = "ItemDescription", ResourceType = typeof(resxDbFields))]
        public string ItemDescription { get; set; }
        public int? OrderId { get; set; }
        public bool Active { get; set; }
    }
    public class ItemModel
    {
        [Display(Name = "ItemGUID", ResourceType = typeof(resxDbFields))]
        public Guid ItemGUID { get; set; }
        [Display(Name = "SequencId", ResourceType = typeof(resxDbFields))]
        public int? SequencId { get; set; }
        public byte[] codeISSItemRowVersion { get; set; }
        [Display(Name = "ItemDescription", ResourceType = typeof(resxDbFields))]
        public string ItemDescription { get; set; }
        public bool Active { get; set; }
        public byte[] codeISSItemLanguageRowVersion { get; set; }
        public int? OrderId { get; set; }
    }
}
