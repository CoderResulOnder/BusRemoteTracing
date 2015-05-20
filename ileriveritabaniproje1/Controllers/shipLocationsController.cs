using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ileriveritabaniproje1.Models;

namespace ileriveritabaniproje1.Controllers
{
    public class shipLocationsController : Controller
    {
        private LİmanYatTekneTakipSistemiEntities db = new LİmanYatTekneTakipSistemiEntities();
        private string yer;
        // GET: shipLocations
        public ActionResult Index()
        {
            var shipLocations = db.shipLocations.Include(s => s.ship);
            return View(shipLocations.ToList());
        }

        [HttpPost]
        public ActionResult Search(string Location)
        {
            yer = Location;

            var result = db.shipLocations.Where(x => x.locationName.StartsWith(Location)).ToList();



            List<mekan> lM = new List<mekan>();

            foreach (shipLocation myL in result)
            {
                mekan nM = new mekan()
                {
                    ID = myL.location_id,
                    name = myL.locationName,
                    lat = Convert.ToDouble(myL.location.Latitude),
                    lng = Convert.ToDouble(myL.location.Longitude),
                };

                lM.Add(nM);
            }

            //var sso=Json(result, JsonRequestBehavior.AllowGet);
            var sso = Json(lM, JsonRequestBehavior.AllowGet);
            return sso;
        }


        [HttpPost]
        public ActionResult nameDistance(string name)
        {
            //ismi girilen otobuse en yakın otobusu bulma
            //ismi girilen harfle baslayan otobusler 
            var result1 = db.shipLocations.Where(x => x.locationName.StartsWith(name)).ToList();
            List<mekan> lM = new List<mekan>();
            //int i = 0;
            
            foreach (shipLocation deger in result1)
            {

                var s = "select top(1) g1.*,g1.location.STDistance(g2.location) as a from shipLocations g1,shipLocations g2"+
                    " where g1.location_id!=g2.location_id and g2.location_id=" + deger.location_id + "order by a ";
                var sonuc = db.shipLocations.SqlQuery(s).ToList();
                var s1 = "select top(1) g2.*,g1.location.STDistance(g2.location) as a from shipLocations g1,shipLocations g2" +
                    " where g1.location_id!=g2.location_id and g2.location_id=" + deger.location_id + "order by a ";
                

                //var  aa = "select top (1) g1.location.STDistance(g2.location) as a from shipLocations g1,shipLocations g2" +
                //  "where g2.location_id=" + deger.location_id + " and g1.location_id!=g2.location_id order by a";
                    

              

               
                foreach (shipLocation myL in sonuc)
                {
                    mekan nM = new mekan()
                    {   

                        name1=deger.locationName,
                        lat1 = Convert.ToDouble(deger.location.Latitude),
                        lng1 = Convert.ToDouble(deger.location.Longitude),
                        //uzaklik=(int)aa[i],
                        ID = myL.location_id,
                        name2=myL.locationName,
                        name = deger.locationName+"->en yakın->"+ myL.locationName,
                        lat = Convert.ToDouble(myL.location.Latitude),
                        lng = Convert.ToDouble(myL.location.Longitude),
                    };

                    lM.Add(nM);
                }
                //i++;
            }

            //var sso=Json(result, JsonRequestBehavior.AllowGet);
            var sso = Json(lM, JsonRequestBehavior.AllowGet);
            return sso;
        }
        [HttpPost]

        public ActionResult Distance(string name, string distance = "100")
        {
            double dd;
            if (distance == "") dd = 0;
            else dd = Convert.ToDouble(distance.ToString()) * 1000;
            var result1 = db.shipLocations.Where(x => x.locationName.StartsWith(name)).ToList();


            List<mekan> lM = new List<mekan>();
            


            foreach (shipLocation deger in result1)
            {

                var s = "select g1.*,g1.location.STDistance(g2.location) as a from shipLocations g1,shipLocations g2 "+
                    "where g1.location_id!=g2.location_id and g1.location.STDistance(g2.location)<" + dd +
                    "and g2.location_id=" + deger.location_id + "order by a";
                var sonuc = db.shipLocations.SqlQuery(s).ToList();

                foreach (shipLocation myL in sonuc)
                {
                    mekan nM = new mekan()
                    {

                        name1=deger.locationName,
                        lat1 = Convert.ToDouble(deger.location.Latitude),
                        lng1 = Convert.ToDouble(deger.location.Longitude),
                        ID = myL.location_id,
                        name = myL.locationName,
                        lat = Convert.ToDouble(myL.location.Latitude),
                        lng = Convert.ToDouble(myL.location.Longitude),
                    };

                    lM.Add(nM);
                }
            }

            //var sso=Json(result, JsonRequestBehavior.AllowGet);
            var sso = Json(lM, JsonRequestBehavior.AllowGet);
            return sso;
        }
        // GET: shipLocations
        
        //**********************************************************************************
        [HttpPost]

        public ActionResult ara(string giden)
        {
            //burada bulundugumuz konuma gore ve girdigimiz konuma gore arama işlemi uygulanıyor 
            if (giden == "") return View();
            //Text1="(39.920770, 32.85411)";
            string[] latLongStr = giden.Split(',');

            string latitude_ = latLongStr[0].Substring(1);
            latitude_ = latitude_.Trim();
            Double latitude_1 = Convert.ToDouble(latitude_);

            string longitude_ = latLongStr[1].Replace(")", "");
            longitude_ = longitude_.Trim();
            
            Double longitude_1 = Convert.ToDouble(longitude_);
            var sonuc = db.shipLocations.SqlQuery("select TOP(1) *,location.STDistance(geography::STGeomFromText('POINT(" + latitude_
                + " " + longitude_ + ")',4326))/1000 as a from shipLocations order by a;").ToList();


            List<mekan> lM = new List<mekan>();

            foreach (shipLocation myL in sonuc)
            {
                mekan nM = new mekan()
                {
                    ID = myL.location_id,
                    name = myL.locationName,
                    lat = Convert.ToDouble(myL.location.Latitude),
                    lng = Convert.ToDouble(myL.location.Longitude),
                };

                lM.Add(nM);
            }

            //var sso=Json(result, JsonRequestBehavior.AllowGet);
            var sso = Json(lM, JsonRequestBehavior.AllowGet);
            return sso;



            //   shipLocation result = (shipLocation)db.shipLocations.Where(x => x.location_id==1)  
            //var pointString = string.Format(
            // "POINT({0} {1})",
            // longitude_1.ToString(),
            // latitude_1.ToString());

            //var point = System.Data.Entity.Spatial.DbGeography.FromText(pointString,4326);


            //var a=System.Data.Entity.Spatial.DbGeography.PointFromText("geograpPOINT({0} {1})",latitude_1,longitude_1,4326);


            //var sonuclar = (from i in db.shipLocations select i.location.Distance());

            //string arasnan = yer;
            //var ali = db.shipLocations.Where(a => a.location.Latitude == latitude_1).ToList(); 
            //foreach(var son in sonuclar){
            //if(son.location_id==sonuc.location_id)    
            //if()
            //    }  
            //    }

            //return View(son);
            //}
            //return View(sonuc);
            //else return View();
        }
        //******************************************************************************************
        // GET: shipLocations/Details/5

        public ActionResult ara1()
        {
            //bura tum otobuslerı bulmada kullanılıyor
            var result = db.shipLocations.ToList();




            List<mekan> lM = new List<mekan>();

            foreach (shipLocation myL in result)
            {
                mekan nM = new mekan()
                {
                    ID = myL.location_id,
                    name = myL.locationName,
                    lat = Convert.ToDouble(myL.location.Latitude),
                    lng = Convert.ToDouble(myL.location.Longitude),
                };

                lM.Add(nM);
            }

            //var sso=Json(result, JsonRequestBehavior.AllowGet);
            var sso = Json(lM, JsonRequestBehavior.AllowGet);
            return sso;
        }




        // GET: shipLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            shipLocation shipLocation = db.shipLocations.Find(id);
            if (shipLocation == null)
            {
                return HttpNotFound();
            }
            return View(shipLocation);
        }

        // GET: shipLocations/Create
        public ActionResult Create()
        {
            ViewBag.ship_id = new SelectList(db.ships, "id", "ShipName");
            return View();
        }

        // POST: shipLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "location_id,locationName,location,image,time,ship_id")] shipLocation shipLocation)
        {
            if (ModelState.IsValid)
            {
                db.shipLocations.Add(shipLocation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ship_id = new SelectList(db.ships, "id", "ShipName", shipLocation.ship_id);
            return View(shipLocation);
        }

        // GET: shipLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            shipLocation shipLocation = db.shipLocations.Find(id);
            if (shipLocation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ship_id = new SelectList(db.ships, "id", "ShipName", shipLocation.ship_id);
            return View(shipLocation);
        }

        // POST: shipLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "location_id,locationName,location,image,time,ship_id")] shipLocation shipLocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shipLocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ship_id = new SelectList(db.ships, "id", "ShipName", shipLocation.ship_id);
            return View(shipLocation);
        }

        // GET: shipLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            shipLocation shipLocation = db.shipLocations.Find(id);
            if (shipLocation == null)
            {
                return HttpNotFound();
            }
            return View(shipLocation);
        }

        // POST: shipLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            shipLocation shipLocation = db.shipLocations.Find(id);
            db.shipLocations.Remove(shipLocation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
