using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCalendar.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Calendar
        public ActionResult Index()
        {
            return View();
        }

        public ContentResult Data()
        {
            Models.Events data = new Models.Events();
            Models.EventsTableAdapters.TableTableAdapter dataAdapter = new Models.EventsTableAdapters.TableTableAdapter();

            dataAdapter.Fill(data.Table);

            dataObject result = new dataObject();

            foreach (var item in data.Table)
            {
                EventObj _eventObj = new EventObj();
                _eventObj.id = item.id.ToString();
                _eventObj.start_date = String.Format("{0:yyyy-MM-dd HH:mm}", item.start_date);
                _eventObj.end_date = String.Format("{0:yyyy-MM-dd HH:mm}", item.end_date);
                _eventObj.text = item.text;
                result.data.Add(_eventObj);
            }

            var list = JsonConvert.SerializeObject(result, Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });

            return Content(list, "application/json");            
        }

        public ActionResult Save(FormCollection actionValues)
        {
            String action_type = actionValues["!nativeeditor_status"];
            string ss = actionValues["id"];
            String source_id = actionValues["id"];
            String target_id = source_id;

            MyCalendar.Models.EventsTableAdapters.TableTableAdapter data = new MyCalendar.Models.EventsTableAdapters.TableTableAdapter();
            
            try
            {
                Guid _guid;
                if (!Guid.TryParse(actionValues["id"],out _guid))                
                    _guid = Guid.NewGuid();                
                DateTime _startDate = DateTime.Parse(actionValues["start_date"]);
                DateTime _endDate = DateTime.Parse(actionValues["end_date"]);
                string _text = actionValues["text"];

                switch (action_type)
                {
                    case "inserted":
                        data.Insert1(_guid,_text,_startDate,_endDate);                        
                        break;
                    case "deleted":                        
                        data.Delete1(_guid);
                        break;
                    default: // "updated"                        
                        data.Update1(_guid, _text, _startDate, _endDate, _guid);
                        break;
                }                
                target_id = _guid.ToString() ;
            }
            catch
            {
                action_type = "error";
            }

            return View(new CalendarActionResponseModel(action_type, source_id, target_id));
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore            
        };

        public class dataObject {            
            public List<EventObj> data { get; set; }
            public dataObject() {
                data = new List<EventObj>();
            }
        }

        public class EventObj {
            public string id { get; set; }
            public string start_date { get; set; }
            public string end_date { get; set; }
            public string text { get; set; }
        }

        public class CalendarActionResponseModel
        {
            public String Status;
            public String Source_id;
            public String Target_id;
            public CalendarActionResponseModel(String status,
                   String source_id, String target_id)
            {
                Status = status;
                Source_id = source_id;
                Target_id = target_id;
            }
        }


    }
}