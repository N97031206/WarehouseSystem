using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using WarehouseSystem.Models;
using WarehouseSystem.Models.ViewModel;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;

namespace WarehouseSystem.Controllers
{
    [AuthorizeUser]
    public class OrderController : Controller
    {
        private IInventoryService m_InventoryService;
        private IInventoryDetailService m_InventoryDetailService;
        private IFindGoodsService m_IFindGoodsService;
        private IFindGoodsDetailService m_IFindGoodsDetailService;

        //private ERP_WebService.ERPWebServiceSoapClient m_ERPWebService = new ERP_WebService.ERPWebServiceSoapClient();

        private WA27P6MMWebService.WA27P6MMServiceSoapClient m_ERPWebService = new WA27P6MMWebService.WA27P6MMServiceSoapClient();

        public OrderController(IInventoryService p_IOrderService, IInventoryDetailService p_IOrderDetailService, IFindGoodsService p_IFindGoodsService, IFindGoodsDetailService p_IFindGoodsDetailService)
        {
            m_InventoryService = p_IOrderService;
            m_InventoryDetailService = p_IOrderDetailService;
            m_IFindGoodsService = p_IFindGoodsService;
            m_IFindGoodsDetailService = p_IFindGoodsDetailService;
        }

        public ActionResult InventoryList(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var InventoryNoList = m_InventoryService.GetAll().ToList();

            List<InventoryViewModel> OrderViewModelList = new List<InventoryViewModel>();

            foreach (var InventoryNo in InventoryNoList)
            {
                OrderViewModelList.Add(new InventoryViewModel()
                {
                    Order = InventoryNo
                });
            }

            return View(OrderViewModelList);
        }

        //public ActionResult CreateInventoryNo()
        //{
        //    InventoryDetailViewModel _InventoryDetailViewModel = new InventoryDetailViewModel();

        //    return View(_InventoryDetailViewModel);
        //}

        public ActionResult CreateInventory()
        {
            InventoryDetailViewModel _InventoryDetailViewModel = GetInventoryData();

            if (_InventoryDetailViewModel.InventoryDetail.Count == 0)
            {
                TempData["message"] = "ERP系統查無盤點資料!!";
                return RedirectToAction("InventoryList");
            }

            string OrderID = "";

            var Result = m_InventoryService.Create(ref OrderID);

            if (Result.Success)
            {
                var Order = m_InventoryService.GetBySID(OrderID);

                IResult _Result = new Result(false);

                try
                {
                    for (int i = 0; i < _InventoryDetailViewModel.InventoryDetail.Count; i++)
                    {
                        _InventoryDetailViewModel.InventoryDetail[i].OrderID = OrderID;
                    }

                    _Result = m_InventoryDetailService.InsertTable(_InventoryDetailViewModel.InventoryDetail);
                }
                catch (Exception ex)
                {

                }

                return RedirectToAction("InventoryNoEdit", new { OrderID = OrderID });
            }
            else
            {
                return RedirectToAction("InventoryList");
            }

        }

        private InventoryDetailViewModel GetInventoryData()
        {
            InventoryDetailViewModel _InventoryDetailViewModel = new InventoryDetailViewModel();

            try
            {
                var _DataSet = m_ERPWebService.GetInventoryData();

                if (_DataSet.Tables[0].Rows.Count > 0)
                {
                    var _Table = _DataSet.Tables[0];

                    for (int i = 0; i < _Table.Rows.Count; i++)
                    {
                        _InventoryDetailViewModel.InventoryDetail.Add(new CST_INV_DTL()
                        {
                            WKNo = _Table.Rows[i]["WKNo"].ToString().Trim(),
                            //Fabric = _Table.Rows[i]["Fabric"].ToString(),
                            //ColorCode = _Table.Rows[i]["ColorCode"].ToString(),
                            //Color = _Table.Rows[i]["Color"].ToString(),
                            PalletRFID = _Table.Rows[i]["PalletRFID"].ToString().Trim(),
                            RFID = _Table.Rows[i]["RFID"].ToString().Trim(),
                            Date = _Table.Rows[i]["Date"].ToString().Trim(),
                            InternalOrder = _Table.Rows[i]["InternalOrder"].ToString().Trim(),
                            Length = _Table.Rows[i]["Length"].ToString().Trim(),
                            Location = _Table.Rows[i]["Location"].ToString().Trim(),
                            Lot = _Table.Rows[i]["WKNo"].ToString().Trim() + _Table.Rows[i]["PNo"].ToString().Trim(),
                            LotNo = _Table.Rows[i]["LotNo"].ToString().Trim(),
                            Order = _Table.Rows[i]["Order"].ToString().Trim(),
                            PalletNo = _Table.Rows[i]["PalletNo"].ToString().Trim(),
                            PNo = _Table.Rows[i]["PNo"].ToString().Trim(),
                            Weight = _Table.Rows[i]["Weight"].ToString().Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _InventoryDetailViewModel.ErrorMessage = ex.ToString();
            }
            return _InventoryDetailViewModel;
        }

        public ActionResult InventoryNoEdit(string OrderID)
        {
            if (string.IsNullOrEmpty(OrderID))
            {
                return RedirectToAction("InventoryList");
            }
            else
            {
                var Order = m_InventoryService.GetBySID(OrderID);

                var _TotalCount = m_InventoryDetailService.GetTotalCount(OrderID);

                if (_TotalCount != 0)
                {
                    var quotient = _TotalCount / 1000;
                    var Remainder = _TotalCount % 1000;

                    if (Remainder != 0) quotient++;

                    List<string> ItemList = new List<string>();

                    for (int i = 0; i < quotient; i++)
                    {
                        ItemList.Add(String.Format("{0:0000}", (i * 1000 + 1)) + "-" + String.Format("{0:0000}", (i + 1) * 1000));
                    }

                    var _List = m_InventoryDetailService.GetData(OrderID, 1, 1000).ToList();

                    InventoryDetailViewModel _InventoryDetailViewModel = new InventoryDetailViewModel()
                    {
                        OrderID = OrderID,
                        InventoryDetail = _List,
                        List = ItemList
                    };

                    _InventoryDetailViewModel.OrderNo = Order.OrderNo;

                    return View(_InventoryDetailViewModel);
                }
                else return RedirectToAction("InventoryList");
            }
        }

        public ActionResult UpdateTable(string OrderID, string Range)
        {
            var MyStr = Range.Split('-');

            var Start = Convert.ToInt32(MyStr[0].Trim());

            var End = Convert.ToInt32(MyStr[1].Trim());

            var _List = m_InventoryDetailService.GetData(OrderID, Start, End).ToList();

            InventoryDetailViewModel _InventoryDetailViewModel = new InventoryDetailViewModel()
            {
                InventoryDetail = _List,
            };


            return PartialView("_Partial", _InventoryDetailViewModel);
        }

        public ActionResult FindGoodsList(string MainMenu, string SubMenu, string MinorMenu)
        {
            UpdateSession(MainMenu, SubMenu, MinorMenu);

            var FindGoodsList = m_IFindGoodsService.GetAll().ToList();

            List<FindGoodsViewModel> OrderViewModelList = new List<FindGoodsViewModel>();

            foreach (var FindGood in FindGoodsList)
            {
                OrderViewModelList.Add(new FindGoodsViewModel()
                {
                    Order = FindGood
                });
            }

            return View(OrderViewModelList);
        }

        public ActionResult CreateFindGoodsNo()
        {
            FindGoodsDetailViewModel _FindGoodsDetailViewModel = new FindGoodsDetailViewModel();

            return View(_FindGoodsDetailViewModel);
        }

        [HttpPost]
        public ActionResult FindGoodsNoQuery(string WKNo, string LotNo, string InternalOrder, string Fabric, string Color, string StartTime, string EndTime, string PLot, string Status, string IsInsert)
        {
            FindGoodsDetailViewModel _FindGoodsDetailViewModel = new FindGoodsDetailViewModel();

            if (!String.IsNullOrEmpty(StartTime)) StartTime = StartTime + " 00:00:00";
            if (!String.IsNullOrEmpty(EndTime)) StartTime = StartTime + " 23:59:59";

            var _List = m_IFindGoodsService.Query(WKNo, LotNo, InternalOrder, Fabric, Color, StartTime, EndTime, PLot, Status).ToList();

            _FindGoodsDetailViewModel.WKNo = WKNo;
            _FindGoodsDetailViewModel.LotNo = LotNo;
            _FindGoodsDetailViewModel.InternalOrder = InternalOrder;
            _FindGoodsDetailViewModel.Fabric = Fabric;
            _FindGoodsDetailViewModel.Color = Color;
            _FindGoodsDetailViewModel.StartTime = StartTime;
            _FindGoodsDetailViewModel.EndTime = EndTime;
            _FindGoodsDetailViewModel.PLot = PLot;
            _FindGoodsDetailViewModel.Status = Status;

            if (String.IsNullOrEmpty(IsInsert))
            {
                if (_List.Count > 0)
                {
                    _FindGoodsDetailViewModel.FindGoodsDetail.AddRange(_List);
                    _FindGoodsDetailViewModel.ErrorMessage = "查詢結果：[ 共" + _List.Count.ToString() + " 筆資料 ]";
                }
                else
                {
                    _FindGoodsDetailViewModel.ErrorMessage = "查詢結果：[ 無資料 ]";
                }

                return View("CreateFindGoodsNo", _FindGoodsDetailViewModel);
            }
            else
            {
                string OrderID = "";

                var Result = m_IFindGoodsService.Create(ref OrderID);

                if (Result.Success)
                {
                    var _Order = m_IFindGoodsService.GetBySID(OrderID);

                    var result = m_IFindGoodsDetailService.InsertTable(_List, OrderID);

                    if (result.Success)
                    {
                        string Msg = "尋貨單號[" + _Order.OrderNo + "]：建立成功";

                        if (result.Success) result.Message = Msg;

                        return Json(new { OrderNo = _Order.OrderNo, Result = result });
                    }
                    else
                    {
                        _FindGoodsDetailViewModel.ErrorMessage = result.Message;
                        return View("CreateFindGoodsNo", _FindGoodsDetailViewModel);
                    }
                }
                else
                {
                    _FindGoodsDetailViewModel.ErrorMessage = Result.Message;
                    return View("CreateFindGoodsNo", _FindGoodsDetailViewModel);
                }
            }
        }

        public ActionResult FindGoodsNoEdit(string OrderID)
        {
            if (string.IsNullOrEmpty(OrderID))
            {
                return RedirectToAction("FindGoodsList");
            }
            else
            {
                var Order = m_IFindGoodsService.GetBySID(OrderID);

                var _List = m_IFindGoodsDetailService.GetDataByOrderID(OrderID).ToList();

                if (_List != null)
                {
                    List<vw_Lots> _vw_Lots = new List<vw_Lots>();

                    for (int i = 0; i < _List.Count; i++)
                    {
                        vw_Lots item = new vw_Lots();
                        item.RFID = _List[i].RFID;
                        item.Lot = _List[i].Lot;
                        item.WKNo = _List[i].WKNo;
                        item.LotNo = _List[i].LotNo;
                        item.Order = _List[i].Order;
                        item.InternalOrder = _List[i].InternalOrder;
                        item.Fabric = _List[i].Fabric;
                        item.Color = _List[i].Color;
                        item.Width = _List[i].Width;
                        item.YdWt = _List[i].YdWt;
                        item.Weight = _List[i].Weight;
                        item.Length = _List[i].Length;
                        item.PNo = _List[i].PNo;
                        item.Date = _List[i].Date;
                        item.ParkingBlockName = _List[i].Location;
                        item.PLot = _List[i].Lot;
                        item.Status = _List[i].Status;
                        _vw_Lots.Add(item);
                    }
                    FindGoodsDetailViewModel _FindGoodsDetailViewModel = new FindGoodsDetailViewModel()
                    {
                        FindGoodsDetail = _vw_Lots
                    };

                    _FindGoodsDetailViewModel.OrderNo = Order.OrderNo;

                    return View(_FindGoodsDetailViewModel);
                }
                else return RedirectToAction("FindGoodsList");
            }
        }

        private void UpdateSession(string MainMenu, string SubMenu, string MinorMenu)
        {
            Session["MainMenu"] = MainMenu;
            Session["SubMenu"] = SubMenu;
            Session["MinorMenu"] = MinorMenu;
        }
    }
}