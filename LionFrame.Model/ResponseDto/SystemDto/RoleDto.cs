namespace LionFrame.Model.ResponseDto.SystemDto
{
    public class RoleDto
    {
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }

        /// <summary>
        /// 当前角色能否操作对应权限和删除该角色等
        /// </summary>
        public bool Operable { get; set; }
    }
}
