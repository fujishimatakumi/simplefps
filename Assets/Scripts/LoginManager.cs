using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;

public class LoginManager : MonoBehaviour
{
    [SerializeField] InputField m_loginUserNameField;
    [SerializeField] InputField m_loginPassWordField;
    [SerializeField] InputField m_CreateUserNameField;
    [SerializeField] InputField m_CreatePassWordField;

    static public NCMBUser m_userData { get; private set; }


    public void CreateAcount()
    {
        string createUserName = m_CreateUserNameField.text;
        string createPasWord = m_CreatePassWordField.text;

        CreateAcountAsync(createUserName,createPasWord);
    }

    private void CreateAcountAsync(string userName, string passWord)
    {
        NCMBUser user = new NCMBUser();
        user.UserName = userName;
        user.Password = passWord;

        user.SignUpAsync((NCMBException e) => {

            if (e != null)
            {
                Debug.LogError("error" + e.ErrorMessage);
            }
            else
            {   
                //自動ログインする場合はその処理を書く
            }
        });
    }

    public void Login()
    {
        string userName = m_loginUserNameField.text;
        string passWord = m_loginPassWordField.text;

        LoginAsync(userName,passWord);
    }

    private void LoginAsync(string userName, string passWord)
    {
        NCMBUser.LogInAsync(userName, passWord, (NCMBException e) =>
        {
            if (e != null)
            {
                Debug.LogError("loginerror" + e.ErrorMessage);
            }
            else
            {  
                m_userData = NCMBUser.CurrentUser;
            }
        });
    }

}
