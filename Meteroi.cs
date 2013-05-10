using System;
using System.Collections.Generic;
using System.Text;
using Telnet;

namespace Meteroi
{
    class broad
    {
        private Terminal shell = null;
        bool connect_state = false;
        private void change_state_to_disconnect()
        {
            connect_state = false;
        }
        private void change_state_to_connect()
        {
            connect_state = true;
        }
        broad(string broad_address, string login_name, string login_pwd, string program)
        {
            string f;
            shell = new Terminal(broad_address);
            shell.Connect(); // physcial connection
            do 
			{
			    f = shell.WaitForString("Login");
			    if (f==null) 
				    break; // this little clumsy line is better to watch in the debugger
                shell.SendResponse(login_name, true);	// send username
                f = shell.WaitForString("Password");
                if (f == null) 
				    break;
                shell.SendResponse(login_pwd, true);	// send password 
                f = shell.WaitForString("$");			// bash
                if (f == null) 
				    break;
                shell.SendResponse(program, true);	// send password 
                f = shell.WaitForString(">");			// program shell
                if (f == null)
                    break;
                change_state_to_connect();
            } while (false);
            change_state_to_disconnect();
        }
        public bool is_connect()
        {
            return connect_state;
        }
        public string send_command_get_response(string command)
        {
            string f;
            shell.SendResponse(command, true);	// send password 
            f = shell.WaitForString(">");			   // program shell
            if (f == null) {
                change_state_to_disconnect();
                return null;
            }
            return shell.VirtualScreen.Hardcopy().TrimEnd();
        }
        ~broad()
        { 
            
        }
    }
}
