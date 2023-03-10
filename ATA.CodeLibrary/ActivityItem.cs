//#region namespaces

//using system;
//using system.collections.generic;
//using system.configuration;
//using system.data;
//using system.data.sqlclient;
//using system.text;
//using susqtech.data.dataobjects;
//using system.web.security;

//using ata.authentication;
//using ata.authentication.providers;
//using ata.member.util;
//using microsoft.applicationblocks.data;

//#endregion

//namespace ata.objectmodel
//{
//    [dataobject("p_activity_create", "p_activity_update", "p_activity_load", "p_activity_delete")]
//    public class activityitem : dataobjectbase
//    {



//        public activityitem()
//        {
//            this.activityid = dataobjectbase.nullintrowid;
//        }

//        public activityitem(int activityid)
//        {
//            this.load(activityid);

//            this.getactivity(activityid);
//        }



//        #region public properties

//        [dataobjectproperty("activityid", sqldbtype.int, true)]
//        public int activityid
//        {
//            get; set;
//        }

//        [dataobjectproperty("activitydate", sqldbtype.datetime)]
//        public datetime activitydate
//        {
//            get; set;
//        }

//        [dataobjectproperty("activitysubject", sqldbtype.nvarchar, 1000)]
//        public string activitysubject
//        {
//            get; set;
//        }

//        [dataobjectproperty("activitynotes", sqldbtype.ntext)]
//        public string activitynotes
//        {
//            get;
//            set;
//        }



//        [dataobjectproperty("activityuserid", sqldbtype.int)]
//        public int activityuserid
//        {
//            get; set;
//        }

//        [dataobjectproperty("onbehalfofuserid", sqldbtype.int)]
//        public int onbehalfofuserid
//        {
//            get; set;
//        }


//        [dataobjectproperty("activitymediacontactid", sqldbtype.int)]
//        public int activitymediacontactid
//        {
//            get; set;
//        }


//        [dataobjectproperty("activitymediaoutletid", sqldbtype.int)]
//        public int activitymediaoutletid
//        {
//            get; set;
//        }

//        [dataobjectproperty("objectiveid", sqldbtype.int)]
//        public int objectiveid
//        {
//            get; set;
//        }


//        [dataobjectproperty("initiativeid", sqldbtype.int)]
//        public int initiativeid
//        {
//            get; set;
//        }

//        [dataobjectproperty("activitytopicid", sqldbtype.int)]
//        public int activitytopicid
//        {
//            get;
//            set;
//        }


//        [dataobjectproperty("activitytypeid", sqldbtype.int)]
//        public int activitytypeid
//        {
//            get;
//            set;
//        }


//        [dataobjectproperty("createddate", sqldbtype.datetime)]
//        public datetime createddate
//        {
//            get;
//            set;
//        }

//        [dataobjectproperty("modifieduserid", sqldbtype.int)]
//        public int modifieduserid
//        { get; set; }



//        [dataobjectproperty("modifieddate", sqldbtype.datetime)]
//        public datetime modifieddate
//        {
//            get;
//            set;
//        }

//        [dataobjectproperty("attachment1", sqldbtype.nvarchar, 1000)]
//        public string attachment1
//        {
//            get;
//            set;
//        }

//        [dataobjectproperty("attachment2", sqldbtype.nvarchar, 1000)]
//        public string attachment2
//        {
//            get;
//            set;
//        }

//        [dataobjectproperty("attachment3", sqldbtype.nvarchar, 1000)]
//        public string attachment3
//        {
//            get;
//            set;
//        }



//        public dictionary<int, int> activitytypes
//        {
//            get;
//            set;
//        }

//        public dictionary<int, int> activitytopics
//        {
//            get;
//            set;
//        }


//        #endregion





//        #region databinding for activity 

//        public static datatable getusernamebyuserid(int userid)
//        {

//            sqlparameter[] sqlparams = new sqlparameter[0];
//            datatable query = new datatable();

//            dataset ds = sqlhelper.executedataset(connectionfactory.dataobjectconnectionstring, commandtype.text, "select [userid],[username],[firstname],[lastname] from  [ata_membership].[dbo].[user] where userid = " + userid + "", sqlparams);

//            if (ds.tables.count > 0)
//            {
//                query = ds.tables[0];
//            }

//            return query;
//        }


//        public static datatable getuseridbyusername(string username)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[0];
//            datatable query = new datatable();

//            dataset ds = sqlhelper.executedataset(connectionfactory.dataobjectconnectionstring, commandtype.text, "select [userid],[username],[firstname],[lastname] from  [ata_membership].[dbo].[user] where username = " + username + "", sqlparams);

//            if (ds.tables.count > 0)
//            {
//                query = ds.tables[0];
//            }
//            return query;
//        }


//        public static dictionary<int, string> getallactivitytypes()
//        {
//            return dbutil.getidstringdictionary("select [activitytypeid] ,[activitytypedescription]  from [ata_membership].[dbo].[activitytype]  where active = 1 order by activitytypedescription", false, null);
//        }

//        public static dictionary<int, string> getallactivitytopics()
//        {
//            return dbutil.getidstringdictionary("select [activitytopicid] ,[activitytopicdescription]  from [ata_membership].[dbo].[activitytopic] where active = 1 order by activitytopicdescription", false, null);
//        }

//        public static dictionary<int, string> IsCurrentUserAdminsponsoredruser(int userid)
//        {
//            return dbutil.getidstringdictionary("select [userid],[username] from  [ata_membership].[dbo].[v_usersponsor_all] where sponsorid = " + userid + "order by [username] ", false, null);
//        }

//        public static dictionary<int, string> geta4ausers()
//        {
//            return dbutil.getidstringdictionary("  select [userid],([lastname] + ', ' +[firstname] ) as name from  [ata_membership].[dbo].[user] where isatastaff = 1  order by [lastname] + ', ' +[firstname]  ", false, null);
//        }

//        public static dictionary<int, string> IsCurrentUserAdminsponsoredcontacts(int userid)
//        {
//            return dbutil.getidstringdictionary("select [userid],([name]  + ' - ' +[company]) as username from  [ata_membership].[dbo].[v_sponsor groups_mycontacts]  where sponsorid = " + userid + "order by [name]  ", false, null);
//        }

//        public static dictionary<int, string> getobjectivebyyear(int year)
//        {
//            return dbutil.getidstringdictionary("select objectiveid,(substring((title + ' - ' + detail),0,61)+'...')  as objectivedescription from [a4astgplanning].[dbo].[objective] where associatedyear = " + year + "order by title + ' - ' + detail", false, null);
//        }

//        public static dictionary<int, string> getinitiativebyobjective(int objectiveid)
//        {
//            return dbutil.getidstringdictionary("select initiativeid, (substring((title + ' - ' + detail),0,61)+'...')  as initiativedescription  from [a4astgplanning].[dbo].[initiative] where objectiveid = " + objectiveid + "order by title + ' - ' + detail", false, null);
//        }

//        public int save()
//        {
//            sqlparameter[] sqlparams = new sqlparameter[12];
//            sqlparams[0] = new sqlparameter("@activitydate", activitydate);
//            sqlparams[1] = new sqlparameter("@activitysubject", activitysubject);
//            sqlparams[2] = new sqlparameter("@activityuserid", activityuserid);
//            if (onbehalfofuserid == 0)
//            {
//                sqlparams[3] = new sqlparameter("@onbehalfofuserid", system.data.sqltypes.sqlint32.null);
//            }
//            else
//            {
//                sqlparams[3] = new sqlparameter("@onbehalfofuserid", onbehalfofuserid);
//            }
//            sqlparams[4] = new sqlparameter("@activitymediacontactid", activitymediacontactid);

//            if (objectiveid == 0)
//            {
//                sqlparams[5] = new sqlparameter("@objectiveid", system.data.sqltypes.sqlint32.null);
//            }
//            else
//            {
//                sqlparams[5] = new sqlparameter("@objectiveid", objectiveid);
//            }

//            if (initiativeid == 0)
//            {
//                sqlparams[6] = new sqlparameter("@initiativeid", system.data.sqltypes.sqlint32.null);
//            }
//            else
//            {
//                sqlparams[6] = new sqlparameter("@initiativeid", initiativeid);
//            }

//            sqlparams[7] = new sqlparameter("@attachment1", attachment1);
//            sqlparams[8] = new sqlparameter("@attachment2", attachment2);
//            sqlparams[9] = new sqlparameter("@attachment3", attachment3);
//            sqlparams[10] = new sqlparameter("@activitynotes", activitynotes);

//            sqlparams[11] = new sqlparameter("@activityid", sqldbtype.int);
//            sqlparams[11].direction = parameterdirection.output;

//            sqlhelper.executenonquery(connectionfactory.getconnection(), commandtype.storedprocedure, "p_activity_create", sqlparams);

//            return convert.toint32(sqlparams[11].value);
//        }


//        public void getactivity(int activityid)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[1];

//            sqlparams[0] = new sqlparameter("@activityid", activityid);


//            datatable query = new datatable();
//            dataset ds = sqlhelper.executedataset(connectionfactory.dataobjectconnectionstring, commandtype.text, "select * from activity where activityid = @activityid ", sqlparams);

//            if (ds.tables.count > 0)
//            {
//                query = ds.tables[0];
//            }

//            this.activityid = activityid;
//            this.activitydate = convert.todatetime(query.rows[0]["activitydate"].tostring());
//            this.activitymediacontactid = convert.toint16(query.rows[0]["activitymediacontactid"].tostring());
//            this.activityuserid = convert.toint16(query.rows[0]["activityuserid"].tostring());

//            int onbehalfofuserid = 0;
//            if (int32.tryparse(query.rows[0]["onbehalfofuserid"].tostring(), out onbehalfofuserid))
//            {
//                this.onbehalfofuserid = onbehalfofuserid;
//            }
//            else
//            {
//                this.onbehalfofuserid = 0;

//            }

//            int objectiveid = 0;
//            if (int32.tryparse(query.rows[0]["objectiveid"].tostring(), out objectiveid))
//            {
//                this.objectiveid = objectiveid;
//            }
//            else
//            {
//                this.objectiveid = 0;

//            }


//            int initiativeid = 0;
//            if (int32.tryparse(query.rows[0]["initiativeid"].tostring(), out initiativeid))
//            {
//                this.initiativeid = initiativeid;
//            }
//            else
//            {
//                this.initiativeid = 0;

//            }

//            this.activitysubject = query.rows[0]["activitysubject"].tostring();
//            this.activitynotes = query.rows[0]["activitynotes"].tostring();
//            this.attachment1 = query.rows[0]["attachment1"].tostring();
//            this.attachment2 = query.rows[0]["attachment2"].tostring();
//            this.attachment3 = query.rows[0]["attachment3"].tostring();



//            this.createddate = convert.todatetime(query.rows[0]["createddate"].tostring());

//            string input1 = query.rows[0]["modifieddate"].tostring(); // dd-mm-yyyy  
//            datetime d;
//            if (datetime.tryparseexact(input1, "dd-mm-yyyy", system.globalization.cultureinfo.invariantculture, system.globalization.datetimestyles.none, out d))
//            {
//                this.modifieddate = d;
//            }
//            else
//            {
//                this.modifieddate = default(datetime);
//            }

//            int modifiedbyid = 0;
//            if (int32.tryparse(query.rows[0]["modifieduserid"].tostring(), out modifiedbyid))
//            {
//                this.modifieduserid = modifiedbyid;
//            }
//            else
//            {
//                this.modifieduserid = 0;

//            }

//        }

//        public void update()
//        {
//            sqlparameter[] sqlparams = new sqlparameter[12];
//            sqlparams[0] = new sqlparameter("@activitydate", activitydate);
//            sqlparams[1] = new sqlparameter("@activitysubject", activitysubject);
//            sqlparams[2] = new sqlparameter("@modifieduserid", modifieduserid);
//            sqlparams[3] = new sqlparameter("@onbehalfofuserid", onbehalfofuserid);
//            sqlparams[4] = new sqlparameter("@activitymediacontactid", activitymediacontactid);
//            sqlparams[5] = new sqlparameter("@objectiveid", objectiveid);
//            sqlparams[6] = new sqlparameter("@initiativeid", initiativeid);
//            sqlparams[7] = new sqlparameter("@attachment1", attachment1);
//            sqlparams[8] = new sqlparameter("@attachment2", attachment2);
//            sqlparams[9] = new sqlparameter("@attachment3", attachment3);
//            sqlparams[10] = new sqlparameter("@activitynotes", activitynotes);
//            sqlparams[11] = new sqlparameter("@activityid", activityid);



//            sqlhelper.executenonquery(connectionfactory.getconnection(), commandtype.storedprocedure, "p_activity_update", sqlparams);

//        }

//        public void saveactivitytypes(int activityid, int activitytypeid)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[3];
//            sqlparams[0] = new sqlparameter("@activityid", activityid);
//            sqlparams[1] = new sqlparameter("@activitytypeid", activitytypeid);

//            sqlparams[2] = new sqlparameter("@id", sqldbtype.int);
//            sqlparams[2].direction = parameterdirection.output;


//            sqlhelper.executenonquery(connectionfactory.getconnection(), commandtype.storedprocedure, "p_activity_activitytype_create", sqlparams);
//        }

//        public void saveactivitytopics(int activityid, int activitytopicid)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[3];
//            sqlparams[0] = new sqlparameter("@activityid", activityid);
//            sqlparams[1] = new sqlparameter("@activitytopicid", activitytopicid);

//            sqlparams[2] = new sqlparameter("@id", sqldbtype.int);
//            sqlparams[2].direction = parameterdirection.output;

//            sqlhelper.executenonquery(connectionfactory.getconnection(), commandtype.storedprocedure, "p_activity_activitytopic_create", sqlparams);
//        }


//        public static int caneditactivity(int editorid, int activityuserid, int onbehalfofuserid)
//        {

//            string sql = "  select count(*) from [ata_membership].[dbo].sponsorgroupuser where userid =  @editorid and sponsorgroupid in (select sponsorgroupid from [ata_membership].[dbo].sponsorgroupuser where userid =  @activityuserid  or  userid =  @onbehalfofuserid)";
//            sqlparameter[] sqlparams = new sqlparameter[3];
//            sqlparams[0] = new sqlparameter("@editorid", editorid);
//            sqlparams[1] = new sqlparameter("@activityuserid", activityuserid);
//            sqlparams[2] = new sqlparameter("@onbehalfofuserid", onbehalfofuserid);

//            object result = sqlhelper.executescalar(conf.connectionstring, commandtype.text, sql, sqlparams);
//            if (result == null)
//                return dataobjectbase.nullintrowid;
//            return (int)result;
//        }


//        public static int canuserviewactivitytab(int userid)
//        {

//            sqlparameter[] sqlparams = new sqlparameter[1];
//            sqlparams[0] = new sqlparameter("@userid", userid);

//            object result = sqlhelper.executescalar(conf.connectionstring, commandtype.storedprocedure, "p_activitytabvisible", sqlparams);

//            if (result == null)
//                return dataobjectbase.nullintrowid;
//            return (int)result;
//        }


//        public static dictionary<int, string> getactivity_activitytype(int activityid)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[1];
//            sqlparams[0] = new sqlparameter("@activityid", activityid);

//            return dbutil.getidstringdictionary("select b.[activitytypeid] ,b.[activitytypedescription] from [ata_membership].[dbo].activity_activitytype a inner join  [ata_membership].[dbo].activitytype b on a.activitytypeid = b.activitytypeid where a.activityid= " + activityid + "order by b.[activitytypedescription]", false, null);

//        }

//        public static dictionary<int, string> getactivity_activitytopic(int activityid)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[1];
//            sqlparams[0] = new sqlparameter("@activityid", activityid);

//            return dbutil.getidstringdictionary("select b.[activitytopicid] ,b.[activitytopicdescription] from [ata_membership].[dbo].activity_activitytopic a inner join [ata_membership].[dbo].activitytopic b on a.activitytopicid = b.activitytopicid where a.activityid= " + activityid + "order by b.[activitytopicdescription]", false, null);

//        }


//        public static datatable getactivityall()
//        {
//            sqlparameter[] sqlparams = new sqlparameter[0];

//            datatable query = new datatable();
//            dataset ds = sqlhelper.executedataset(connectionfactory.dataobjectconnectionstring, commandtype.text, "select * from [v_activityfull] order by [activityid] desc", sqlparams);

//            return ds.tables[0];
//        }

//        public static datatable getactivitybyusername(string username)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[0];

//            datatable query = new datatable();
//            dataset ds = sqlhelper.executedataset(connectionfactory.dataobjectconnectionstring, commandtype.text, "select * from [v_activitybycontact]  where [created by] like '" + username + "' or onbehalfofusername like '" + username + "' or contact like '" + username + "'  order by [activity date] desc", sqlparams);

//            return ds.tables[0];
//        }

//        public void delete(int activityid)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[1];
//            sqlparams[0] = new sqlparameter("@activityid", activityid);
//            sqlhelper.executenonquery(connectionfactory.getconnection(), commandtype.text, "delete from activity where activityid=@activityid", sqlparams);
//        }

//        public void deleteactivity_activitytopic(int activityid)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[1];
//            sqlparams[0] = new sqlparameter("@activityid", activityid);
//            sqlhelper.executenonquery(connectionfactory.getconnection(), commandtype.text, "delete from activity_activitytopic where activityid=@activityid", sqlparams);
//        }

//        public void deleteactivity_activitytype(int activityid)
//        {
//            sqlparameter[] sqlparams = new sqlparameter[1];
//            sqlparams[0] = new sqlparameter("@activityid", activityid);
//            sqlhelper.executenonquery(connectionfactory.getconnection(), commandtype.text, "delete from activity_activitytype where activityid=@activityid", sqlparams);
//        }



//        #endregion
//    }

//}


