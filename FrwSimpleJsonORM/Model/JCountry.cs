/**********************************************************************************
 *   FrwSimpleWinCRUD   https://github.com/frwsoftware/FrwSimpleWinCRUD
 *   The Open-Source Library for most quick  WinForm CRUD application creation
 *   MIT License Copyright (c) 2016 FrwSoftware
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
namespace FrwSoftware
{
    [JDisplayName(typeof(FrwUtilsRes), "JCountry_Country")]
    [JEntity]
    public class JCountry
    {
        [JDisplayName(typeof(FrwUtilsRes), "JCountry_Identifier")]
        [JPrimaryKey]
        public string JCountryId { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JCountry_Name")]
        [JNameProperty, JRequired, JUnique]
        public string Name { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JCountry_Flag")]
        [JImageName(DisplyPropertyStyle.TextAndImage)]
        public string Image { get; set; } ////https://www.iconfinder.com/icons/167745/ua_icon#size=23 flags

        public string Official_name_en { get; set; }
        public string Official_name_fr { get; set; }
        public string ISO3166_1_Alpha_2 { get; set; }
        public string ISO3166_1_Alpha_3 { get; set; }
        public string M49 { get; set; }
        public string ITU { get; set; }
        public string MARC { get; set; }
        public string WMO { get; set; }
        public string DS { get; set; }
        public string Dial { get; set; }
        public string FIFA { get; set; }
        public string FIPS { get; set; }
        public string GAUL { get; set; }
        public string IOC { get; set; }
        public string ISO4217_currency_alphabetic_code { get; set; }
        public string ISO4217_currency_country_name { get; set; }
        public string ISO4217_currency_minor_unit { get; set; }
        public string ISO4217_currency_name { get; set; }
        public string ISO4217_currency_numeric_code { get; set; }
        public string Is_independent { get; set; }
        public string Capital { get; set; }
        public string Continent { get; set; }
        public string TLD { get; set; }
        public string Languages { get; set; }
        public string Geoname_ID { get; set; }
        public string EDGAR { get; set; }
        public string WikidataId { get; set; }
    }
}
