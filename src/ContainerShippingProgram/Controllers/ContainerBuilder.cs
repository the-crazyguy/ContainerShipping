using ContainerShippingProgram.ContainerEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Controllers
{
    //TODO: Just make the sub-container properties public ffs...
    public class ContainerBuilder
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string OriginCountry { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public bool? IsRefridgerated { get; set; }

        public ContainerBuilder()
        {
            ResetObject();
        }

        public void ResetObject()
        {
            Id = null;
            Type = string.Empty;
            Description = string.Empty;
            OriginCountry = string.Empty;
            Weight = null;
            Volume = null;
            IsRefridgerated = null;
        }

        public BaseContainer GetContainer()
        {
            if (Id == null)
            {
                //TODO: make an exception for incomplete container
                throw new ArgumentNullException();
            }

            switch (Type)
            {
                case ProtocolMessages.FullType:
                    if (Weight == null || IsRefridgerated == null)
                    {
                        //TODO: make an exception for incomplete container
                        throw new ArgumentNullException();
                    }
                    return new FullContainer(Id.Value, Description, OriginCountry, Weight.Value, IsRefridgerated.Value);

                case ProtocolMessages.HalfType:
                    if (Volume == null)
                    {
                        //TODO: make an exception for incomplete container
                        throw new ArgumentNullException();
                    }
                    return new HalfContainer(Id.Value, Description, OriginCountry, Volume.Value);

                case ProtocolMessages.QuartType:
                    return new QuartContainer(Id.Value, Description, OriginCountry);
                
                default:
                    // TODO: Make an exception
                    throw new ArgumentException();
            }
        }
    }
}
