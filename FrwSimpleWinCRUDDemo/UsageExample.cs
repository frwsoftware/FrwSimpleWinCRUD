using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrwSoftware.Model.Example;

namespace FrwSoftware
{
    class UsageExample
    {
        static void ExampleOdDataManipulation()
        {
            //get all data
            IList<JExampleDto> list = Dm.Instance.FindAll<JExampleDto>();
            IList list2 = Dm.Instance.FindAll(typeof(JExampleDto));

            //find by params
            JExampleDto dtofinded = Dm.Instance.FindAll<JExampleDto>().FirstOrDefault<JExampleDto>(c => c.Field4 == "www");

            //find, create (empty), insert, update
            object pk = "1";
            JExampleDto dto = Dm.Instance.FindByPrimaryKey<JExampleDto>(pk);
            if (dto == null)
            {
                dto = Dm.Instance.EmptyObject<JExampleDto>(null);
            }
            Dm.Instance.InsertOrUpdateObject(dto);

            JExampleDto dto1 = (JExampleDto)Dm.Instance.FindByPrimaryKey(typeof(JExampleDto), pk);
            if (dto1 == null)
            {
                dto1 = (JExampleDto)Dm.Instance.EmptyObject(typeof(JExampleDto), null);
            }
            Dm.Instance.InsertOrUpdateObject(dto1);

            //remove 
            Dm.Instance.DeleteObject(dto);

            //remove all
            Dm.Instance.DeleteAllObjects(typeof(JExampleDto));

            /////////////////
            //relations 
            ////////////////
            //many to one 
            //resolve - auto
            //access fields 
            string d = dto.JExampleDto2.Field1;
            //set 
            JExampleDto2 dto2 = (JExampleDto2)Dm.Instance.FindByPrimaryKey(typeof(JExampleDto2), pk);
            dto.JExampleDto2 = dto2;
            Dm.Instance.InsertOrUpdateObject(dto);
            //uset
            dto.JExampleDto2 = null;
            Dm.Instance.InsertOrUpdateObject(dto);


            /////////////////////////////////
            //one to many
            JExampleDto2 dto3 = Dm.Instance.FindByPrimaryKey<JExampleDto2>("3");
            JExampleDto2 dto4 = Dm.Instance.FindByPrimaryKey<JExampleDto2>("4");
            JExampleDto2 dto5 = Dm.Instance.FindByPrimaryKey<JExampleDto2>("5");
            dto.JExampleDto2s.Add(dto3);
            dto.JExampleDto2s.Add(dto4);
            dto.JExampleDto2s.Add(dto5);
            Dm.Instance.InsertOrUpdateObject(dto);
            //unset
            dto.JExampleDto2s.Remove(dto5);
            Dm.Instance.InsertOrUpdateObject(dto);
            //uset all
            dto.JExampleDto2s.Clear();
            Dm.Instance.InsertOrUpdateObject(dto);

            /////////////////////////////////
            //many to many
            //same as one to many 
            //set
            dto.JExampleDto2ss.Add(dto5);
            Dm.Instance.InsertOrUpdateObject(dto);
            //unset
            dto.JExampleDto2ss.Remove(dto5);
            Dm.Instance.InsertOrUpdateObject(dto);
            //uset all
            dto.JExampleDto2ss.Clear();
            Dm.Instance.InsertOrUpdateObject(dto);

        }

    }
}
