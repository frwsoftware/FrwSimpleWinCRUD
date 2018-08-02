/**********************************************************************************
 *   WinForms HTML Editor https://code.msdn.microsoft.com/windowsapps/WinForms-HTML-Editor-01dbce1a   
 *   MS-LPL 
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
#region Using directives

using System;

#endregion

namespace MSDN.Html.Editor
{

    /// <summary>
    /// Enum used to insert a list
    /// </summary>
    public enum HtmlListType
    {
        Ordered,
        Unordered

    } //HtmlListType


    /// <summary>
    /// Enum used to insert a heading
    /// </summary>
    public enum HtmlHeadingType
    {
        H1 = 1,
        H2 = 2,
        H3 = 3,
        H4 = 4,
        H5 = 5

    } //HtmlHeadingType


    /// <summary>
    /// Enum used to define the navigate action on a user clicking a href
    /// </summary>
    public enum NavigateActionOption
    {
        Default,
        NewWindow,
        SameWindow

    } //NavigateActionOption


    /// <summary>
    /// Enum used to define the image align property
    /// </summary>
    public enum ImageAlignOption
    {
        AbsBottom,
        AbsMiddle,
        Baseline,
        Bottom,
        Left,
        Middle,
        Right,
        TextTop,
        Top

    } //ImageAlignOption


    /// <summary>
    /// Enum used to define the text alignment property
    /// </summary>
    public enum HorizontalAlignOption
    {
        Default,
        Left,
        Center,
        Right

    } //HorizontalAlignOption


    /// <summary>
    /// Enum used to define the vertical alignment property
    /// </summary>
    public enum VerticalAlignOption
    {
        Default,
        Top,
        Bottom

    } //VerticalAlignOption


    /// <summary>
    /// Enum used to define the visibility of the scroll bars
    /// </summary>
    public enum DisplayScrollBarOption
    {
        Yes,
        No,
        Auto

    } //DisplayScrollBarOption


    /// <summary>
    /// Enum used to define the unit of measure for a value
    /// </summary>
    public enum MeasurementOption
    {
        Pixel,
        Percent

    } //MeasurementOption

}