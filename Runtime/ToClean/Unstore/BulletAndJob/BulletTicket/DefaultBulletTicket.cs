public class DefaultBulletTicket :IBulletIdTicket {
    private int m_id;

    public DefaultBulletTicket(int id)
    {
        m_id = id;
    }
    public void GetId(out int id)
    {
        id = m_id;
    }
}