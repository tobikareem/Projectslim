
namespace Slim.Pages.Pages
{
    public static class ManageClassAttr
    {
        public static string Add_d_noneAnd_d_lg_block(int index)
        {
            return index > 3 && index % 4 >= 1 ? "d-none d-lg-block" : "";
        }
    }
}
