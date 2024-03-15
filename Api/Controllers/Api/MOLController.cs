using Api.Dtos;
using Api.Models;
using Api.ServiceMOL;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.ServiceModel.Channels;
using System.IO;
using System.IO.Compression;
using Api.DataModel;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/MOL")]
    public class MOLController : ApiController
    {
        private static Entities _context;

        public MOLController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        private static Opzioni GetOpzioni(tipoStampa tipoStampa, fronteRetro fronteRetro, ricevutaRitorno ricevutaRitorno)
        {
            var opzioni = new Opzioni();

            var opzioniStampa = new OpzioniStampa();
            opzioniStampa.TipoColore = (tipoStampa == tipoStampa.colori ? TipoColore.COLORE : TipoColore.BW);
            opzioniStampa.FronteRetro = (fronteRetro == fronteRetro.fronteRetro ? true : false);

            var servizio = new OpzioniServizio();
            servizio.Consegna = ModalitaConsegna.S;
            servizio.AttestazioneConsegna = false;

            if (ricevutaRitorno == ricevutaRitorno.si)
                servizio.AttestazioneConsegna = true;

            opzioni.Stampa = opzioniStampa;
            opzioni.Servizio = servizio;

            return opzioni;
        }

        private static RaccomandataMarketServiceClient getNewServiceMOL(Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var Users = _context.Users.Where(a => a.guidUser == guidUser).SingleOrDefault(a => a.parentId == 0);
            if (Users == null)
                return null;
            RaccomandataMarketServiceClient service = new RaccomandataMarketServiceClient();
            if (Users.areaTestUser)
            {
                service.ClientCredentials.UserName.UserName = Users.usernamePosteAreaTest;
                service.ClientCredentials.UserName.Password = Users.pwdPosteAreaTest;
            }
            else
            {
                service.ClientCredentials.UserName.UserName = Users.usernamePoste;
                service.ClientCredentials.UserName.Password = Users.pwdPoste;
            }
            return service;
        }

        private static Mittente GetMittente(SenderDto sender, string businessName)
        {
            Mittente m = new Mittente();

            m.Nominativo = businessName;
            m.ComplementoIndirizzo = "";
            m.ComplementoNominativo = "";
            m.Indirizzo = sender.dug + " " + sender.address + " " + sender.houseNumber;
            m.Cap = sender.cap;
            m.Comune = sender.city;
            m.Provincia = sender.province;
            m.Nazione = sender.state;

            return m;
        }


        public static AvvisoPagamentoPagoPA getBollettinoPA(BulletinsDtos b, Users u)
        {
            string[] codiceAvvisi = b.CodiciAvvisi.Split('/');
            var scadenza = b.Scadenza.Split('/')[1] + "/" + b.Scadenza.Split('/')[0] + "/" + b.Scadenza.Split('/')[2];

            var ap = new AvvisoPagamentoPagoPA();

            ap.EnteCreditore = new AvvisoPagamentoPagoPAEnteCreditore
            {
                DenominazioneEnte = u.denominazioneEntePA,
                InfoEnte = u.infoEntePA,
                SettoreEnte = u.settoreEntePA,
                AutorizzazioneStampaInProprio = "AutorizzazioneStampaInProprio",
                CodiceFiscaleEnte = u.codiceFiscaleEntePA,
                LogoEnte = File.ReadAllBytes(HttpContext.Current.Server.MapPath(u.logoPA))
            };

            ap.Destinatario = new AvvisoPagamentoPagoPADestinatario
            {
                CodiceFiscaleDestinatario = b.codiceCliente,
                IndirizzoDestinatario = b.eseguitoDaIndirizzo + " " + b.eseguitoDaCAP + " " + b.eseguitoDaLocalita,
                NomeCognomeDestinatario = b.eseguitoDaNominativo
            };

            int numeroRate = codiceAvvisi.Length;

            var pagamento = new AvvisoPagamentoPagoPAPagamento
            {
                NumeroCCPostale = b.numeroContoCorrente,
                IntestatarioCCPostale = b.intestatoA,
                CBILL = b.CBILL,

                DelTuoEnte = "di PosteItaliane o",
                RateTesto = numeroRate > 1 ? " oppure in " + numeroRate + " rate" : "",
                CodiceAvviso = codiceAvvisi[0],
                Oggetto = b.causale,
                Importo = b.importoEuro,
                Scadenza = Convert.ToDateTime(scadenza),
            };
            ap.Pagamento = pagamento;

            if (numeroRate > 1)
            {
                decimal importoRata = 0;
                switch (numeroRate)
                {
                    case 2:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[0],
                            Importo = importoRata,
                            NumeroRata = "1",
                            Scadenza = pagamento.Scadenza
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[1],
                            Importo = Math.Round(pagamento.Importo - importoRata, 2),
                            NumeroRata = "2",
                            Scadenza = pagamento.Scadenza.AddDays(31)
                        },
                        };
                        break;
                    case 3:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[0],
                            Importo = importoRata,
                            NumeroRata = "1",
                            Scadenza = pagamento.Scadenza
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[1],
                            Importo = importoRata,
                            NumeroRata = "2",
                            Scadenza = pagamento.Scadenza.AddDays(31)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[2],
                            Importo = Math.Round(pagamento.Importo - (importoRata * 2), 2),
                            NumeroRata = "3",
                            Scadenza = pagamento.Scadenza.AddDays(62)
                        },
                        };
                        break;
                    case 4:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[0],
                            Importo = importoRata,
                            NumeroRata = "1",
                            Scadenza = pagamento.Scadenza
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[1],
                            Importo = importoRata,
                            NumeroRata = "2",
                            Scadenza = pagamento.Scadenza.AddDays(31)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[2],
                            Importo = importoRata,
                            NumeroRata = "3",
                            Scadenza =pagamento.Scadenza.AddDays(62)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[3],
                            Importo = Math.Round(pagamento.Importo - (importoRata * 3), 2),
                            NumeroRata = "4",
                            Scadenza = pagamento.Scadenza.AddDays(93)
                        },
                        };
                        break;
                    case 5:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[0],
                            Importo = importoRata,
                            NumeroRata = "1",
                            Scadenza = pagamento.Scadenza
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[1],
                            Importo = importoRata,
                            NumeroRata = "2",
                            Scadenza = pagamento.Scadenza.AddDays(31)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[2],
                            Importo = importoRata,
                            NumeroRata = "3",
                            Scadenza = pagamento.Scadenza.AddDays(62)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[3],
                            Importo = importoRata,
                            NumeroRata = "4",
                            Scadenza = pagamento.Scadenza.AddDays(93)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[4],
                            Importo = Math.Round(pagamento.Importo - (importoRata * 4), 2),
                            NumeroRata = "5",
                            Scadenza = pagamento.Scadenza.AddDays(124)
                        },
                            };
                        break;
                    case 6:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[0],
                            Importo = importoRata,
                            NumeroRata = "1",
                            Scadenza = pagamento.Scadenza
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[1],
                            Importo = importoRata,
                            NumeroRata = "2",
                            Scadenza =pagamento.Scadenza.AddDays(31)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[2],
                            Importo = importoRata,
                            NumeroRata = "3",
                            Scadenza = pagamento.Scadenza.AddDays(62)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[3],
                            Importo = importoRata,
                            NumeroRata = "4",
                            Scadenza = pagamento.Scadenza.AddDays(93)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[4],
                            Importo = importoRata,
                            NumeroRata = "5",
                            Scadenza = pagamento.Scadenza.AddDays(124)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[5],
                            Importo = Math.Round(pagamento.Importo - (importoRata * 5), 2),
                            NumeroRata = "6",
                            Scadenza = pagamento.Scadenza.AddDays(155)
                        },
                    };
                        break;
                    case 7:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[0],
                            Importo = importoRata,
                            NumeroRata = "1",
                            Scadenza = pagamento.Scadenza
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[1],
                            Importo = importoRata,
                            NumeroRata = "2",
                            Scadenza =pagamento.Scadenza.AddDays(31)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[2],
                            Importo = importoRata,
                            NumeroRata = "3",
                            Scadenza = pagamento.Scadenza.AddDays(62)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[3],
                            Importo = importoRata,
                            NumeroRata = "4",
                            Scadenza = pagamento.Scadenza.AddDays(93)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[4],
                            Importo = importoRata,
                            NumeroRata = "5",
                            Scadenza = pagamento.Scadenza.AddDays(124)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[5],
                            Importo = importoRata,
                            NumeroRata = "6",
                            Scadenza =pagamento.Scadenza.AddDays(155)
                        },
                        new AvvisoPagamentoPagoPAPagamentoRata
                        {
                            CodiceAvvisoRata = codiceAvvisi[6],
                            Importo = Math.Round(pagamento.Importo - (importoRata * 6), 2),
                            NumeroRata = "7",
                            Scadenza =pagamento.Scadenza.AddDays(186)
                        },
                            };
                        break;
                    case 8:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[0],
                                Importo = importoRata,
                                NumeroRata = "1",
                                Scadenza =pagamento.Scadenza
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[1],
                                Importo = importoRata,
                                NumeroRata = "2",
                                Scadenza =pagamento.Scadenza.AddDays(31)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[2],
                                Importo = importoRata,
                                NumeroRata = "3",
                                Scadenza = pagamento.Scadenza.AddDays(62)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[3],
                                Importo = importoRata,
                                NumeroRata = "4",
                                Scadenza =pagamento.Scadenza.AddDays(93)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[4],
                                Importo = importoRata,
                                NumeroRata = "5",
                                Scadenza = pagamento.Scadenza.AddDays(124)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[5],
                                Importo = importoRata,
                                NumeroRata = "6",
                                Scadenza = pagamento.Scadenza.AddDays(155)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[6],
                                Importo = importoRata,
                                NumeroRata = "7",
                                Scadenza = pagamento.Scadenza.AddDays(186)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[7],
                                Importo = Math.Round(pagamento.Importo - (importoRata * 7), 2),
                                NumeroRata = "8",
                                Scadenza = pagamento.Scadenza.AddDays(217)
                            },
                    };
                        break;
                    case 9:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[0],
                                Importo = importoRata,
                                NumeroRata = "1",
                                Scadenza =pagamento.Scadenza
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[1],
                                Importo = importoRata,
                                NumeroRata = "2",
                                Scadenza = pagamento.Scadenza.AddDays(31)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[2],
                                Importo = importoRata,
                                NumeroRata = "3",
                                Scadenza = pagamento.Scadenza.AddDays(62)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[3],
                                Importo = importoRata,
                                NumeroRata = "4",
                                Scadenza =pagamento.Scadenza.AddDays(93)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[4],
                                Importo = importoRata,
                                NumeroRata = "5",
                                Scadenza =pagamento.Scadenza.AddDays(124)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[5],
                                Importo = importoRata,
                                NumeroRata = "6",
                                Scadenza =pagamento.Scadenza.AddDays(155)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[6],
                                Importo = importoRata,
                                NumeroRata = "7",
                                Scadenza = pagamento.Scadenza.AddDays(186)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[7],
                                Importo = importoRata,
                                NumeroRata = "8",
                                Scadenza = pagamento.Scadenza.AddDays(217)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[8],
                                Importo = Math.Round(pagamento.Importo - (importoRata * 8), 2),
                                NumeroRata = "9",
                                Scadenza = pagamento.Scadenza.AddDays(248)
                            },
                        };
                        break;
                    case 10:
                        importoRata = Math.Round(pagamento.Importo / numeroRate, 2);
                        ap.Pagamento.Rate = new AvvisoPagamentoPagoPAPagamentoRata[]
                        {
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[0],
                                Importo = importoRata,
                                NumeroRata = "1",
                                Scadenza =pagamento.Scadenza
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[1],
                                Importo = importoRata,
                                NumeroRata = "2",
                                Scadenza = pagamento.Scadenza.AddDays(31)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[2],
                                Importo = importoRata,
                                NumeroRata = "3",
                                Scadenza = pagamento.Scadenza.AddDays(62)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[3],
                                Importo = importoRata,
                                NumeroRata = "4",
                                Scadenza = pagamento.Scadenza.AddDays(93)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[4],
                                Importo = importoRata,
                                NumeroRata = "5",
                                Scadenza =pagamento.Scadenza.AddDays(124)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[5],
                                Importo = importoRata,
                                NumeroRata = "6",
                                Scadenza = pagamento.Scadenza.AddDays(155)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[6],
                                Importo = importoRata,
                                NumeroRata = "7",
                                Scadenza =pagamento.Scadenza.AddDays(186)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[7],
                                Importo = importoRata,
                                NumeroRata = "8",
                                Scadenza = pagamento.Scadenza.AddDays(217)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[8],
                                Importo = importoRata,
                                NumeroRata = "9",
                                Scadenza =pagamento.Scadenza.AddDays(248)
                            },
                            new AvvisoPagamentoPagoPAPagamentoRata
                            {
                                CodiceAvvisoRata = codiceAvvisi[9],
                                Importo = Math.Round(pagamento.Importo - (importoRata * 9), 2),
                                NumeroRata = "10",
                                Scadenza =pagamento.Scadenza.AddDays(279)
                            },
                    };
                        break;
                }
            }

            return ap;
        }


        public static Bollettino896 getBollettino896(BulletinsDtos bollettino)
        {
            Bollettino896 b = new Bollettino896();
            b.NumeroContoCorrente = bollettino.numeroContoCorrente;
            b.IntestatoA = bollettino.intestatoA;
            b.EseguitoDa = new BollettinoEseguitoDa();
            b.EseguitoDa.Nominativo = bollettino.eseguitoDaNominativo;
            b.EseguitoDa.Indirizzo = bollettino.eseguitoDaIndirizzo;
            b.EseguitoDa.CAP = bollettino.eseguitoDaCAP;
            b.EseguitoDa.Localita = bollettino.eseguitoDaLocalita;
            b.CodiceCliente = bollettino.codiceCliente;
            b.ImportoEuro = bollettino.importoEuro;
            b.Causale = bollettino.causale;
            if (bollettino.IBAN != null && bollettino.IBAN != "")
                b.IBAN = bollettino.IBAN;
            b.Template = "896";
            return b;
        }

        public static Bollettino451 getBollettino451(BulletinsDtos bollettino)
        {
            Bollettino451 b = new Bollettino451();
            b.NumeroContoCorrente = bollettino.numeroContoCorrente;
            b.IntestatoA = bollettino.intestatoA;
            b.FormatoStampa = 0;
            b.AdditionalInfo = "";
            b.IBAN = "";
            b.EseguitoDa = new BollettinoEseguitoDa();
            b.EseguitoDa.Nominativo = bollettino.eseguitoDaNominativo;
            b.EseguitoDa.Indirizzo = bollettino.eseguitoDaIndirizzo;
            b.EseguitoDa.CAP = bollettino.eseguitoDaCAP;
            b.EseguitoDa.Localita = bollettino.eseguitoDaLocalita;
            b.ImportoEuro = bollettino.importoEuro;
            b.Causale = bollettino.causale;
            return b;
        }

        public static Bollettino674 getBollettino674(BulletinsDtos bollettino)
        {
            Bollettino674 b = new Bollettino674();
            b.NumeroContoCorrente = bollettino.numeroContoCorrente;
            b.IntestatoA = bollettino.intestatoA;
            b.FormatoStampa = 0;
            b.AdditionalInfo = "";
            b.IBAN = "";
            b.EseguitoDa = new BollettinoEseguitoDa();
            b.EseguitoDa.Nominativo = bollettino.eseguitoDaNominativo;
            b.EseguitoDa.Indirizzo = bollettino.eseguitoDaIndirizzo;
            b.EseguitoDa.CAP = bollettino.eseguitoDaCAP;
            b.EseguitoDa.Localita = bollettino.eseguitoDaLocalita;
            b.CodiceCliente = bollettino.codiceCliente;
            b.Causale = bollettino.causale;
            return b;
        }

        public static Documento[] getDoc(List<string> strNomeFile, int NumeroDiDocumenti)
        {
            System.IO.FileInfo file;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            Documento[] ArrayDocumento = new Documento[NumeroDiDocumenti - 1 + 1];
            for (var i = 0; i <= NumeroDiDocumenti - 1; i++)
            {
                Documento documento = new Documento();
                var immagine = System.IO.File.ReadAllBytes(strNomeFile[i]);
                documento.Estensione = "pdf";
                documento.Contenuto = immagine;
                documento.MD5 = System.BitConverter.ToString(md5.ComputeHash(immagine)).Replace("-", string.Empty);
                ArrayDocumento[i] = documento;
            }
            return ArrayDocumento;
        }

        private void createFeatures(tipoStampa tipoStampa, fronteRetro fronteRetro, ricevutaRitorno ricevutaRitorno, int operationId)
        {
            var op1 = new operationFeatures();
            op1.operationId = operationId;
            op1.featureType = "tipo stampa";
            op1.featureValue = Enum.GetName(typeof(tipoStampa), tipoStampa);
            _context.operationFeatures.Add(op1);
            _context.SaveChanges();

            var op2 = new operationFeatures();
            op2.operationId = operationId;
            op2.featureType = "fronte Retro";
            op2.featureValue = Enum.GetName(typeof(fronteRetro), fronteRetro);
            _context.operationFeatures.Add(op2);
            _context.SaveChanges();

            var op3 = new operationFeatures();
            op3.operationId = operationId;
            op3.featureType = "ricevuta Ritorno";
            op3.featureValue = Enum.GetName(typeof(ricevutaRitorno), ricevutaRitorno);
            _context.operationFeatures.Add(op3);
            _context.SaveChanges();

        }

        private static Destinatario GetDestinatarioMOL(NamesDto name, int? index = 1)
        {
            Destinatario n = new Destinatario();
            n.Nominativo = name.businessName + " " + name.surname + " " + name.name;
            n.ComplementoNominativo = name.complementNames;
            n.ComplementoIndirizzo = name.complementAddress;
            n.Indirizzo = name.dug + " " + name.address + " " + name.houseNumber;
            n.Cap = name.cap;
            n.Comune = name.city;
            n.Provincia = name.province;
            n.Nazione = name.state;

            return n;
        }

        private static DestinatarioAR GetDestinatarioARMOL(SenderDto name, string businessName, int? index = 1)
        {
            DestinatarioAR n = new DestinatarioAR();
            n.Nominativo = businessName;
            n.ComplementoNominativo = "";
            n.ComplementoIndirizzo = "";
            n.Indirizzo = name.dug + " " + name.address + " " + name.houseNumber;
            n.Cap = name.cap;
            n.Comune = name.city;
            n.Provincia = name.province;
            n.Nazione = name.state;

            return n;
        }

        [Route("GetState")]
        [HttpGet]
        public ResponseMOLState GetState(string requestId, Guid guidUser)
        {
            var user = _context.Users.Where(a => a.guidUser == guidUser).FirstOrDefault();

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaStatoRequest();
            request.CodiceContratto = user.CodiceContrattoMOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseMOLState();

            var rs = service.RecuperaStato(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                var s = rs.StatoInvii[0];
                r.stato = s.CodiceStatoRichiesta;
                r.descrizioneStato = s.DescrizioneStatoRichiesta;
                r.dataUltimaModifica = s.DataUltimaModifica.ToString();

                var l = GlobalClass.ListOfState();
                var state = l.SingleOrDefault(a => a.identificativo == s.CodiceStatoRichiesta);

                if (state != null)
                {
                    if (state.tipologia.ToUpper() != "DEFINITIVO")
                    {
                        var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                        if (!state.state)
                            n.valid = false;

                        n.stato = s.DescrizioneStatoRichiesta;
                        _context.SaveChanges();
                    }
                }
            }
            return r;
        }

        [Route("StateRetrive")]
        [HttpGet]
        public ResponseMOLConfirm StateRetrive(string requestId, Guid guidUser)
        {
            var user = _context.Users.Where(a => a.guidUser == guidUser).FirstOrDefault();

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaServizioPerIdRichiestaRequest();
            request.CodiceContratto = user.CodiceContrattoMOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseMOLConfirm();

            var rs = service.RecuperaServizioPerIdRichiesta(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                if (rs.Servizi.Count() > 0)
                {
                    var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                    n.presaInCaricoDate = rs.Servizi[0].DataAccettazione;
                    n.codice = rs.Servizi[0].DatiServizio.Destinatari[0].NumeroRaccomandata.Replace(" ", "");
                    n.stato = rs.Servizi[0].StatoServizio;
                    _context.SaveChanges();

                    r.DataAccettazione = (DateTime)rs.Servizi[0].DataAccettazione;
                    r.NumeroRaccomandata = rs.Servizi[0].DatiServizio.Destinatari[0].NumeroRaccomandata.Replace(" ", "");
                    r.EsitoPostaEvo = rs.Esito;
                }
            }
            return r;
        }

        [Route("ResultRetrive")]
        [HttpGet]
        public ResponseMOLConfirm ResultRetrive(string requestId, Guid guidUser)
        {
            var user = _context.Users.Where(a => a.guidUser == guidUser).FirstOrDefault();

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaEsitiPerIdRichiestaRequest();
            request.CodiceContratto = user.CodiceContrattoMOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseMOLConfirm();

            var rs = service.RecuperaEsitiPerIdRichiesta(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                r.EsitoPostaEvo = rs.Esito;
                r.NumeroRaccomandata = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                r.DataAccettazione = rs.RendicontazioneEsiti[0].DataAccettazione;

                var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                n.presaInCaricoDate = rs.RendicontazioneEsiti[0].DataAccettazione;
                n.codice = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                _context.SaveChanges();

            }
            return r;
        }

        [Route("StateRetriveForCode")]
        [HttpGet]
        public ResponseMOLConfirm StateRetriveForCode(string code, Guid guidUser)
        {
            var user = _context.Users.Where(a => a.guidUser == guidUser).FirstOrDefault();

            string[] codes = new string[1];
            codes[0] = code;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaServizioPerNumeroRaccomandataRequest();
            request.CodiceContratto = user.CodiceContrattoMOL;
            request.NumeroRaccomandate = codes;


            var r = new ResponseMOLConfirm();

            var rs = service.RecuperaServizioPerNumeroRaccomandata(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                //r.EsitoPostaEvo = rs.Esito;
                //r.NumeroRaccomandata = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                //r.DataAccettazione = rs.RendicontazioneEsiti[0].DataAccettazione;

                //var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                //n.presaInCaricoDate = rs.RendicontazioneEsiti[0].DataAccettazione;
                //n.codice = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                //_context.SaveChanges();

            }
            return r;
        }

        [Route("Confirm")]
        [HttpGet]
        public ResponseMOLConfirm Confirm(string requestId, Guid guidUser)
        {
            var user = _context.Users.Where(a => a.guidUser == guidUser).FirstOrDefault();

            var r = new ResponseMOLConfirm();
            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new ConfermaInvioRequest();
            request.CodiceContratto = user.CodiceContrattoMOL;
            request.IdRichiesta = requestId;

            var stato = StateRetrive(requestId, guidUser);
            if (stato.EsitoPostaEvo == EsitoPostaEvo.OK)
            {
                r.DataAccettazione = (DateTime)stato.DataAccettazione;
                r.NumeroRaccomandata = stato.NumeroRaccomandata.Replace(" ", "");
                r.EsitoPostaEvo = stato.EsitoPostaEvo;
            }
            else
            {
                var conferma = service.ConfermaInvio(request);
                if (conferma.Esito == EsitoPostaEvo.OK)
                {
                    r.DataAccettazione = conferma.DataAccettazione;
                    r.NumeroRaccomandata = conferma.DestinatariRaccomandate[0].NumeroRaccomandata;
                    r.EsitoPostaEvo = conferma.Esito;

                    var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                    n.presaInCaricoDate = r.DataAccettazione;
                    n.codice = r.NumeroRaccomandata;
                    n.stato = "Presa in carico Postel";
                    n.currentState = (int)currentState.PresoInCarico;
                    _context.SaveChanges();
                }
            }

            return r;
        }

        [Route("CheckAllFiles")]
        [HttpPost]
        public async Task<GetNumberOfCheckedNames> CheckAllFiles([FromUri] Guid guidUser, [FromBody] ObjectSubmit senderRecipients, [FromUri] bool tsc,
            [FromUri] bool frc, [FromUri] bool rrc, [FromUri] int userId)
        {
            SenderDto sender = new SenderDto();
            sender = senderRecipients.sender;
            List<GetRecipent> GetRecipents = senderRecipients.recipients;

            GetNumberOfCheckedNames ncn = new GetNumberOfCheckedNames();
            List<GetCheckedNames> lgcn = new List<GetCheckedNames>();

            //MULTIPLE USERS
            var users = _context.Users.Where(a => a.guidUser == guidUser);

            //ERRORE GUID
            if (users.Count() == 0)
            {
                ncn.numberOfValidNames = 0;
                ncn.state = "Utente non riconosiuto";
                return ncn;
            }

            //UTENTE INSERITORE
            var u = new Users();
            if (userId > 0)
                u = users.SingleOrDefault(a => a.id == userId);
            else
                u = users.SingleOrDefault(a => a.parentId == 0);


            //ERRORE MITTENTE
            ControlloMittente ctrlM = GlobalClass.verificaMittente(senderRecipients.sender);
            if (!ctrlM.Valido)
            {
                ncn.numberOfValidNames = 0;
                ncn.state = "Mittente non valido";
                return ncn;
            }

            //ERRORE MITTENTE AR
            if (senderRecipients.senderAR != null)
            {
                ControlloMittente ctrlMAR = GlobalClass.verificaMittente(senderRecipients.senderAR);
                if (!ctrlMAR.Valido)
                {
                    ncn.numberOfValidNames = 0;
                    ncn.state = "Destinatario AR non valido";
                    return ncn;
                }
            }

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.MOL;
            op.demoOperation = u.demoUser;
            op.areaTestOperation = u.areaTestUser;
            op.complete = false;
            op.csvFileName = senderRecipients.csvFile;

            int operationId = OperationsController.CreateItem(op);

            tipoStampa ts = tipoStampa.colori;
            if (tsc)
                ts = tipoStampa.biancoNero;

            fronteRetro fr = fronteRetro.fronte;
            if (!frc)
                fr = fronteRetro.fronteRetro;

            ricevutaRitorno rr = ricevutaRitorno.si;
            if (!rrc)
                rr = ricevutaRitorno.no;


            createFeatures(ts, fr, rr, operationId);

            SenderDtos ss = Mapper.Map<SenderDto, SenderDtos>(sender);
            ss.operationId = operationId;
            int senderId = SenderController.CreateItem(ss);

            //CREAZIONE MITTENTE AR
            if (senderRecipients.senderAR != null)
            {
                SenderDtos ssAR = Mapper.Map<SenderDto, SenderDtos>(senderRecipients.senderAR);
                ssAR.operationId = operationId;
                ssAR.AR = true;
                int senderIdAR = SenderController.CreateItem(ssAR);
            }

            int validNames = 0;
            foreach (var GetRecipent in GetRecipents.ToList())
            {
                int id = (int)GetRecipent.recipient.id;
                var b = _context.Bulletins.Where(a => a.namesListsId == id).ToList();
                if (b.Count() > 0)
                {
                    GetRecipent.bulletin = Mapper.Map<Bulletins, BulletinsDtos>(b[0]);
                };

                NamesDtos nos = Mapper.Map<NamesDto, NamesDtos>(GetRecipent.recipient);
                nos.operationId = operationId;
                nos.operationType = op.operationType;
                if (!nos.state.ToUpper().Contains("ITALIA") && !nos.state.ToUpper().Contains("ITA"))
                    nos.operationType = (int)operationType.LOL;
                nos.requestId = null;
                nos.guidUser = null;
                nos.valid = true;

                nos.fronteRetro = Convert.ToBoolean(fr);
                nos.ricevutaRitorno = Convert.ToBoolean(rr);
                nos.tipoStampa = Convert.ToBoolean(ts);

                nos.insertDate = DateTime.Now;
                nos.currentState = (int)currentState.inAttesa;

                var nc = new NamesController();
                int idName = nc.CreateItem(nos, u.userPriority);
                if (GetRecipent.bulletin != null)
                {
                    BulletinsDto bos = Mapper.Map<BulletinsDtos, BulletinsDto>(GetRecipent.bulletin);
                    bos.namesId = idName;
                    BulletinsController.CreateItem(bos);
                }
                validNames++;

                GetCheckedNames gcn = new GetCheckedNames()
                {
                    name = nos,
                    valid = true,
                    price = new Prices()
                };

                lgcn.Add(gcn);

            }

            ncn.numberOfValidNames = validNames;
            ncn.checkedNames = lgcn;
            ncn.state = "Inserimento valido!";
            ncn.valid = true;
            ncn.operationId = operationId;

            return ncn;

        }


        [Route("RequestOperationStatus")]
        [HttpGet]
        public List<GetStatoRichiesta> RequestOperationStatus(Guid guidUser, int operationId)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            List<GetStatoRichiesta> lGsr = new List<GetStatoRichiesta>();
            var n = _context.Names.Where(a => a.operationId == operationId).Where(a => a.requestId != null).ToList();
            if (n.Count() == 0)
                return null;

            int id = n[0].operationId;
            var o = _context.Operations.SingleOrDefault(a => a.id == id);

            foreach (var name in n)
            {
                var s = GetState(name.requestId, guidUser);
                GetStatoRichiesta gsr = new GetStatoRichiesta();
                gsr.requestId = name.requestId;

                gsr.statoDescrizione = s.descrizioneStato;
                gsr.numeroServizio = name.codice;
                gsr.dataEsito = s.dataUltimaModifica;

                //name.consegnatoDate = Convert.ToDateTime(gsr.dataEsito);

                lGsr.Add(gsr);
            }

            return lGsr;
        }

        [Route("RequestNameStatus")]
        [HttpGet]
        public GetStatoRichiesta RequestNameStatus(Guid guidUser, Names name)
        {

            var s = GetState(name.requestId, guidUser);
            GetStatoRichiesta gsr = new GetStatoRichiesta();
            gsr.requestId = name.requestId;

            gsr.statoDescrizione = s.descrizioneStato;
            gsr.numeroServizio = name.codice;
            gsr.dataEsito = s.dataUltimaModifica;

            return gsr;
        }

        [Route("RequestDCS")]
        [HttpGet]
        public string RequestDCS(Guid guidUser, int id)
        {
            var user = _context.Users.Where(a => a.guidUser == guidUser).FirstOrDefault();

            RaccomandataMarketServiceClient service = new RaccomandataMarketServiceClient();
            service = getNewServiceMOL(guidUser);

            var name = _context.Names.SingleOrDefault(a => a.id == id);

            RecuperaDocumentoRequest request = new RecuperaDocumentoRequest();
            request.CodiceContratto = user.CodiceContrattoMOL;
            request.IdRichiesta = name.requestId;

            var dcs = service.RecuperaDocumento(request);
            var nameFile = "";

            if (dcs.Esito == EsitoPostaEvo.OK)
            {
                var n = DateTime.Now.Ticks + ".pdf";
                nameFile = "/public/download/" + n;
                var path = HttpContext.Current.Server.MapPath(nameFile);
                System.IO.File.WriteAllBytes(path, dcs.Documento.Contenuto);

                //SALVATAGGIO TEMPORANEO FILE
                //FINO A SETT/OTT 2020
                name.pathRecoveryFile = nameFile;
                _context.SaveChanges();

                GlobalClass.SendViaFtp(path, n, GlobalClass.usernameFtpDoc, GlobalClass.passwordFtpDoc, GlobalClass.ftpUrlDoc);
                File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("/public/download/"), n));
            }

            return nameFile;
        }

        private static Documento[] GetDocInsert(byte[] bite)
        {
            System.IO.FileInfo file;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            Documento[] ArrayDocumento = new Documento[1];
            Documento documento = new Documento();
            file = new System.IO.FileInfo("");
            documento.Estensione = "pdf";
            documento.Contenuto = bite;
            documento.MD5 = System.BitConverter.ToString(md5.ComputeHash(documento.Contenuto)).Replace("-", string.Empty);
            ArrayDocumento[0] = documento;

            return ArrayDocumento;
        }

        //NEW SEND
        [Route("SendNames")]
        [HttpGet]
        public async Task<Names> sendNames(GetRecipent GetRecipent, int operationId, SenderDto sender, int userId, bool autoconfirm = true)
        {
            var n = _context.Names.SingleOrDefault(a => a.id == GetRecipent.recipient.id);

            var user = _context.Users.SingleOrDefault(a => a.id == userId);
            var guidUser = user.guidUser;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);

            var request = GetInvioRequest(user.CodiceContrattoMOL, sender, user.businessName, GetRecipent, ProdottoPostaEvo.MOL4, ModalitaConsegna.S, false, false, user);
            request.MarketOnline.AutoConferma = autoconfirm;

            var j = Newtonsoft.Json.JsonConvert.SerializeObject(request);

            try
            {
                var response = await service.InvioAsync(request);

                switch (response.Esito)
                {
                    case EsitoPostaEvo.OK:
                        n.requestId = response.IdRichiesta;
                        n.stato = "Accettata on line";
                        n.currentState = (int)currentState.AccettatoOnline;
                        break;
                    case EsitoPostaEvo.KO:
                        n.stato = response.Errori[0].Messaggio.ToString();
                        n.currentState = (int)currentState.ErroreSubmit;
                        n.valid = false;
                        break;
                }

            }
            catch (Exception e)
            {
                n.valid = false;
                n.stato = e.InnerException.ToString();
                n.currentState = (int)currentState.ErroreSubmit;

                //GESTIONE ECCEZIONE
                //CREAZIONE LOG
                GlobalClass.CreateTxtFile("Errore durante l'invio MOL\n"
                    + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\n" + e.Message.ToString() + "\n", "INVIO-" + DateTime.Now.ToString("dd-MM-yyy"), "MOL");

            };

            _context.SaveChanges();
            return n;
        }

        [Route("RetriveState")]
        [HttpGet]
        public async Task<bool> RetriveState(List<Names> list, int userId)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(a => a.id == userId);
                var guidUser = user.guidUser;

                Thread.Sleep(15000);

                RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);

                foreach (var n in list)
                {
                    var wait = false;
                    var idRichiesta = n.requestId;

                    var response = await service.RecuperaStatoAsync(new RecuperaStatoRequest
                    {
                        IdRichieste = new[] { idRichiesta },
                        CodiceContratto = user.CodiceContrattoMOL
                    });

                    if (response.Esito == EsitoPostaEvo.KO)
                        wait = true;

                    if (!wait && response.StatoInvii[0].CodiceStatoRichiesta == "L")
                        wait = true;

                    int i = 0;
                    if (!wait)
                        do
                        {
                            Thread.Sleep(5000);

                            response = await service.RecuperaStatoAsync(new RecuperaStatoRequest
                            {
                                IdRichieste = new[] { idRichiesta },
                                CodiceContratto = user.CodiceContrattoMOL
                            });

                            wait = response.Esito == EsitoPostaEvo.OK && response.StatoInvii[0].CodiceStatoRichiesta != "L";
                            i++;
                            if (response.StatoInvii[0].CodiceStatoRichiesta == "N")
                                wait = false;
                        } while (wait && i < 50);

                    var nn = _context.Names.SingleOrDefault(a => a.requestId == n.requestId);

                    switch (response.Esito)
                    {
                        case EsitoPostaEvo.KO:
                            nn.currentState = (int)currentState.ErroreConfirm;
                            nn.stato = response.Errori[0].Messaggio.ToString();
                            nn.valid = false;
                            break;

                        case EsitoPostaEvo.OK:
                            nn.stato = response.StatoInvii[0].DescrizioneStatoRichiesta.Replace("Postel", "Poste");
                            if (response.StatoInvii[0].CodiceStatoRichiesta != "L")
                            {
                                nn.currentState = (int)currentState.ErroreStatoAtteso;
                                nn.valid = false;
                            }
                            else
                            {
                                nn.currentState = (int)currentState.PresoInCarico;
                            }
                            break;

                    }

                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {

                //GESTIONE ECCEZIONE
                //CREAZIONE LOG
                GlobalClass.CreateTxtFile("Errore durante recupera stato MOL\n"
                    + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\n" + e.Message.ToString() + "\n", "RECUPERA_STATO-" + DateTime.Now.ToString("dd-MM-yyy"), "MOL");

                return false;
            }
            return true;
        }

        [Route("RetriveService")]
        [HttpGet]
        public async Task<List<ResponseMOLState>> RetriveService(List<Names> list, int userId)
        {
            var r = new List<ResponseMOLState>();
            var user = _context.Users.SingleOrDefault(a => a.id == userId);
            var guidUser = user.guidUser;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            foreach (var name in list)
            {
                var services = await service.RecuperaServizioPerIdRichiestaAsync(
                    new RecuperaServizioPerIdRichiestaRequest
                    {
                        CodiceContratto = user.CodiceContrattoMOL,
                        IdRichieste = new[] { name.requestId }
                    });

                if (services.Servizi != null)
                {
                    var postalizzata = services.Servizi[0];
                    var IdRichiesta = postalizzata.DatiServizio.IdRichiesta;
                    var n = _context.Names.SingleOrDefault(a => a.requestId == IdRichiesta);
                    var rl = new ResponseMOLState()
                    {
                        dataUltimaModifica = postalizzata.DataAccettazione.Value.ToString(),
                        descrizioneStato = postalizzata.StatoServizio.Replace("Postel", "Poste"),
                        stato = "L"
                    };
                    r.Add(rl);

                    //Recupero il Numero Lettera
                    var numeroLettera = postalizzata.DatiServizio.Destinatari[0].NumeroRaccomandata;
                    if (postalizzata.ValorizzazioneServizio != null)
                    {
                        var ImportoNettoServizio = postalizzata.ValorizzazioneServizio.ImportoNettoServizio;
                        n.price = Convert.ToDecimal(ImportoNettoServizio);
                        n.totalPrice = Convert.ToDecimal(ImportoNettoServizio);
                    }
                    n.stato = postalizzata.StatoServizio.Replace("Postel", "Poste");
                    n.currentState = (int)currentState.InLavorazione;
                    n.codice = numeroLettera;
                    _context.SaveChanges();

                    //SALVATAGGIO TEMPORANEO FILE
                    //FINO A SETT/OTT 2020
                    if (n.pathRecoveryFile == null)
                        RequestDCS(n.Operations.Users.guidUser, n.id);

                }
            }
            return r;
        }

        [Route("SetNamesStateMOL")]
        [HttpGet]

        public async Task<bool> SetNamesStateMOL(Guid guidUser, string requestId)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return false;

            var n = _context.Names
                .Where(a => a.finalState != true)
                .SingleOrDefault(a => a.requestId == requestId);
            if (n == null)
                return false;

            RaccomandataMarketServiceClient service = new RaccomandataMarketServiceClient();
            service = getNewServiceMOL(guidUser);


            var s = await service.RecuperaEsitiPerIdRichiestaAsync(new RecuperaEsitiPerIdRichiestaRequest()
            {
                IdRichieste = new[] { requestId },
                CodiceContratto = u.ToList()[0].CodiceContrattoMOL
            });

            if (s.Esito == EsitoPostaEvo.OK)
            {
                n.finalState = false;
                n.stato = s.RendicontazioneEsiti[0].DescrizioneEsito;
                if (s.RendicontazioneEsiti[0].CodiceEsito == "01" ||
                   s.RendicontazioneEsiti[0].CodiceEsito == "03" ||
                   s.RendicontazioneEsiti[0].CodiceEsito == "04")
                {
                    if (s.RendicontazioneEsiti[0].CodiceEsito == "01")
                        n.consegnatoDate = s.RendicontazioneEsiti[0].DataEsito;

                    n.finalState = true;

                }
                _context.SaveChanges();
            }
            else
            {
                var services = await service.RecuperaServizioPerIdRichiestaAsync(
                    new RecuperaServizioPerIdRichiestaRequest
                    {
                        CodiceContratto = u.ToList()[0].CodiceContrattoCOL,
                        IdRichieste = new[] { requestId }
                    });

                if (services.Servizi != null)
                {
                    var sv = services.Servizi[0];
                    n.stato = sv.StatoServizio.Replace("Postel", "Poste");
                    if (sv.ValorizzazioneServizio != null)
                    {
                        n.price = Convert.ToDecimal(sv.ValorizzazioneServizio.ImportoNettoServizio);
                        n.vatPrice = 0;
                        n.totalPrice = Convert.ToDecimal(sv.ValorizzazioneServizio.ImportoNettoServizio);
                    }

                }
            }
            _context.SaveChanges();
            return true;
        }

        [Route("InvioLotto")]
        [HttpPost]
        public async Task<MolColResponse> InvioLotto(int operationId)
        {
            var res = new MolColResponse();
            var o = _context.Operations.SingleOrDefault(a => a.id == operationId);
            if (o == null)
            {
                res.messaggio = "Operazione non trovata";
                return res;
            };

            var s = o.Senders.SingleOrDefault(a => a.operationId == operationId);
            if (s == null)
            {
                res.messaggio = "Mittente non trovato";
                return res;
            };
            var sender = Mapper.Map<Senders, SenderDto>(s);

            var ns = o.Names
                .Where(a => a.idLotto == null)
                .Where(a => a.operationId == operationId)
                .Take(500);

            if (ns.Count() == 0)
            {
                res.messaggio = "Operazione non trovata";
                return res;
            };

            List<string> files = new List<string>();
            var list = new List<GetRecipent>();
            foreach (var n in ns)
            {
                var gr = new GetRecipent();
                gr.bulletin = null;
                gr.recipient = Mapper.Map<Names, NamesDto>(n);
                list.Add(gr);
                files.Add(n.fileName);
            };

            RaccomandataMarketServiceClient client = getNewServiceMOL(o.Users.guidUser);

            // Nel trasferimento dell'archivio Vanno specificati il CodiceContratto e 
            // l'estensione dell'archivio come Soap Headers
            AddSoapHeader(client, "CodiceContratto", o.Users.CodiceContrattoMOL, "http://comunicazionielettroniche.posteitaliane.it/postaevo/data");
            AddSoapHeader(client, "Estensione", "zip", "http://comunicazionielettroniche.posteitaliane.it/postaevo/data");
            var stream = File.OpenRead(CreateZipFile(files));

            var caricaLottoResponse = await client.CaricaLottoAsync(stream);

            if (caricaLottoResponse == null)
            {
                res.messaggio = "Caricamento lotto non riuscito";
                return res;
            }

            if (caricaLottoResponse.Esito == EsitoPostaEvo.KO)
            {
                res.messaggio = "Caricamento lotto non riuscito";
                return res;
            }

            var idLotto = caricaLottoResponse.IdLotto;

            var invii = new InvioRequest[list.Count];
            for (var i = 0; i < invii.Length; i++)
            {
                invii[i] = GetInvioRequest(o.Users.CodiceContrattoMOL, sender, o.Users.businessName, list[i], ProdottoPostaEvo.MOL4, ModalitaConsegna.S, false, false);
                invii[i].Intestazione.IdentificativoInvioNelLotto = i;
                invii[i].MarketOnline.AutoConferma = true;

                foreach (var document in invii[i].MarketOnline.Documenti)
                {
                    var fileName = list[i].recipient.fileName;
                    List<string> filesName = new List<string>();
                    filesName.Add(fileName);

                    document.Contenuto = getDoc(filesName, 1)[0].Contenuto;
                    document.PercorsoDocumentoLotto = GlobalClass.GetFileName(list[i].recipient.fileName);
                }
            }

            var request = new InvioLottoRequest
            {
                CodiceContratto = o.Users.CodiceContrattoMOL,
                BloccaLottoConErrore = false,
                IdLotto = idLotto,
                Invii = invii
            };

            var response = new InvioLottoResponse();

            try
            {
                response = await client.InvioLottoAsync(request);
            }
            catch (Exception e)
            {
                res.messaggio = "Invio lotto non riuscito";
                return res;
            }
            if (response == null)
            {
                res.messaggio = "Invio lotto non riuscito";
                return res;
            }

            if (response.Esito == EsitoPostaEvo.KO)
            {
                res.messaggio = "Invio lotto non riuscito";
                return res;
            }


            for (var i = 0; i < list.Count; i++)
            {
                int id = (int)list[i].recipient.id;
                var n = _context.Names.SingleOrDefault(a => a.id == id);
                n.idLotto = idLotto;
                n.IndiceNelLotto = i;
                _context.SaveChanges();
            }

            return res;
        }

        [Route("RecuperaServizioLotto")]
        [HttpPost]
        public async Task<MolColResponse> RecuperaServizioPerIdLotto(string idLotto)
        {
            var res = new MolColResponse();
            var n = _context.Names
                .Where(a => a.requestId == null)
                .Where(a => a.codice == null)
                .FirstOrDefault(a => a.idLotto == idLotto);

            if (n == null)
                return res;

            var o = _context.Operations.SingleOrDefault(a => a.id == n.operationId);

            RaccomandataMarketServiceClient client = getNewServiceMOL(o.Users.guidUser);

            var servizioResponse = await client.RecuperaServizioPerIdLottoAsync(
            new RecuperaServizioPerIdLottoRequest
            {
                IdLotto = idLotto,
                CodiceContratto = o.Users.CodiceContrattoMOL,
                NumeroElementiPerPagina = 10,
                NumeroPagina = 1,
            });

            if (servizioResponse.Richieste.Count() == 0)
            {
                res.messaggio = "Nessuna richiesta disponibile";
                return res;
            }
            res.numeroDiInvii = servizioResponse.NumeroTotaleElementi;
            res.codici = (List<string>)servizioResponse.Richieste.Select(a => a.DatiServizio.Destinatari.Select(x => x.NumeroRaccomandata).ToList());

            await SalvaCodiciRaccomandate(servizioResponse.Richieste, idLotto);

            if (servizioResponse.NumeroTotaleElementi == o.Names.Count())
            {
                o.lottoCompleto = true;
                _context.SaveChanges();
            }

            return res;
        }


        [Route("AnnullaStampa")]
        [HttpGet]
        public async Task<bool> AnnullaStampa(string codice)
        {
            var n = _context.Names
                .Where(a => a.requestId != null)
                .FirstOrDefault(a => a.codice == codice);

            if (n == null)
                return false;

            var o = _context.Operations.SingleOrDefault(a => a.id == n.operationId);

            RaccomandataMarketServiceClient client = getNewServiceMOL(o.Users.guidUser);

            var servizioResponse = await client.AnnullaStampaAsync(
            new AnnullaStampaRequest
            {
                CodiceContratto = o.Users.CodiceContrattoMOL,
                IdRichiesta = n.requestId
            });

            return true;
        }


        private string CreateZipFile(List<string> files)
        {
            var zipFile = System.Web.Hosting.HostingEnvironment.MapPath("/Public/ZipFile/file-" + DateTime.Now.Ticks.ToString() + ".zip");
            using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
            {
                foreach (var fPath in files)
                {
                    archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath));
                }
            }
            return zipFile;
        }

        private async Task SalvaCodiciRaccomandate(ServizioDestinatarioRaccomandata[] richieste, string idLotto)
        {
            foreach (var r in richieste.ToList())
            {
                var n = _context.Names
                    .Where(a => a.requestId == null)
                    .Where(a => a.codice == null)
                    .Where(a => a.idLotto == idLotto)
                    .SingleOrDefault(a => a.IndiceNelLotto == r.DatiServizio.IdentificativoInvioNelLotto);
                if (n.id == 0)
                    continue;

                n.requestId = r.DatiServizio.IdRichiesta;
                n.codice = r.DatiServizio.Destinatari[0].NumeroRaccomandata;
                n.currentState = (int)currentState.PresoInCarico;
                _context.SaveChanges();
            }
        }

        private InvioRequest GetInvioRequest(string codiceContratto, SenderDto s, string businessName, GetRecipent GetRecipent,
                                     ProdottoPostaEvo prodotto,
                                     ModalitaConsegna modalitaConsegna,
                                     bool attestazioneConsegna,
                                     bool secondoTentativoRecapito,
                                     Users user = null)
        {

            var request = new InvioRequest
            {
                Intestazione = new Intestazione
                {
                    CodiceContratto = codiceContratto,
                    Prodotto = prodotto
                },

                MarketOnline = new MarketOnline
                {
                    Mittente = GetMittente(s, businessName)
                }
            };

            request.MarketOnline.AutoConferma = false;

            request.MarketOnline.Destinatari = new[]
            {
                GetDestinatarioMOL(GetRecipent.recipient)
            };

            //BOLLETTINO
            if (GetRecipent.bulletin != null)
            {
                if (GetRecipent.bulletin.PagoPA != true)
                {
                    PaginaBollettino pagina = new PaginaBollettino();
                    object b = null;
                    //switch (GetRecipent.bulletin.bulletinType)
                    //{
                    //    case (int)bulletinType.Bollettino451:
                    //        b = getBollettino451(GetRecipent.bulletin);
                    //        pagina.Bollettino = (Bollettino451)b;
                    //        break;
                    //    case (int)bulletinType.Bollettino674:
                    //        b = getBollettino674(GetRecipent.bulletin);
                    //        pagina.Bollettino = (Bollettino674)b;
                    //        break;
                    //    case (int)bulletinType.Bollettino896:
                    //        b = getBollettino896(GetRecipent.bulletin);
                    //        pagina.Bollettino = (Bollettino896)b;
                    //        break;
                    //}
                    b = getBollettino896(GetRecipent.bulletin);
                    pagina.Bollettino = (Bollettino896)b;
                    PaginaBollettino[] p = new PaginaBollettino[1];
                    p[0] = pagina;
                    request.MarketOnline.Bollettini = p;
                }
                else
                {
                    request.MarketOnline.BollettinoPA = getBollettinoPA(GetRecipent.bulletin, user);
                }
            }

            //OPZIONI
            tipoStampa ts = tipoStampa.colori;
            if (GetRecipent.recipient.tipoStampa)
                ts = tipoStampa.biancoNero;

            fronteRetro fr = fronteRetro.fronte;
            if (GetRecipent.recipient.fronteRetro)
                fr = fronteRetro.fronteRetro;

            ricevutaRitorno rr = ricevutaRitorno.no;
            if (GetRecipent.recipient.ricevutaRitorno)
                rr = ricevutaRitorno.si;

            request.MarketOnline.Opzioni = GetOpzioni(ts, fr, rr);

            if (GetRecipent.recipient.ricevutaRitorno)
            {
                var sAR = s;

                var senderAR = _context.Senders
                    .Where(a => a.AR == true)
                    .SingleOrDefault(a => a.operationId == GetRecipent.recipient.operationId);
                if (senderAR != null)
                    sAR = Mapper.Map<Senders, SenderDto>(senderAR);

                request.MarketOnline.DestinatarioAR = GetDestinatarioARMOL(sAR, businessName);
            }


            var fileName = GetRecipent.recipient.fileName;
            List<string> filesName = new List<string>();
            filesName.Add(fileName);

            request.MarketOnline.Documenti = getDoc(filesName, 1);

            return request;
        }

        private void AddSoapHeader(RaccomandataMarketServiceClient client, string key, string value, string nameSpace)
        {
            OperationContextScope c;
            if (OperationContext.Current == null)
                c = new OperationContextScope(client.InnerChannel);

            OperationContext.Current.OutgoingMessageHeaders.Add(MessageHeader.CreateHeader(key, nameSpace, value));
        }

        [Route("TestInvioLotto")]
        [HttpGet]
        public async Task<MolColResponse> TestInvioLotto()
        {
            return await InvioLotto(1437);
        }

        public async Task<List<ResponseMOLState>> SetState(int namesId)
        {
            var r = new List<ResponseMOLState>();
            var nn = _context.Names.FirstOrDefault(a => a.id == namesId);
            if (nn == null)
                return null;


            var o = _context.Operations.FirstOrDefault(a => a.id == nn.operationId);
            if (o == null)
                return null;

            var user = _context.Users.FirstOrDefault(a => a.id == o.userId);
            if (user == null)
                return null;

            var guidUser = user.guidUser;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            try
            {
                var s = await service.RecuperaEsitiPerIdRichiestaAsync(new RecuperaEsitiPerIdRichiestaRequest()
                {
                    IdRichieste = new[] { nn.requestId },
                    CodiceContratto = user.CodiceContrattoMOL
                });

                var n = _context.Names.FirstOrDefault(a => a.id == nn.id);

                if (s.Esito == EsitoPostaEvo.OK)
                {
                    n.finalState = false;
                    n.stato = s.RendicontazioneEsiti[0].DescrizioneEsito;
                    n.codice = s.RendicontazioneEsiti[0].CodiceTracciatura;
                    n.currentState = (int)currentState.InLavorazione;
                    if (s.RendicontazioneEsiti[0].CodiceEsito == "01" ||
                        s.RendicontazioneEsiti[0].CodiceEsito == "03" ||
                        s.RendicontazioneEsiti[0].CodiceEsito == "04")
                    {
                        if (s.RendicontazioneEsiti[0].CodiceEsito == "01")
                            n.consegnatoDate = s.RendicontazioneEsiti[0].DataEsito;

                        if (s.RendicontazioneEsiti[0].CodiceEsito == "03")
                            n.stato = s.RendicontazioneEsiti[0].DescrizioneEsito + " " + s.RendicontazioneEsiti[0].Causale;

                        n.finalState = true;

                        if (s.RendicontazioneEsiti[0].CodiceEsito == "04")
                            n.finalState = false;

                    }
                    else
                    {
                        var rr = s.RendicontazioneEsiti[0].DescrizioneEsito;
                        var cd = n.codice;
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var error = ex;
            }

            return r;
        }

    }
}
