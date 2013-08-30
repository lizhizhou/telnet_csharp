using System;
using System.Collections.Generic;
using System.Text;
using Telnet;
using Meteroi;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;

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
        public broad(string broad_address, string login_name, string login_pwd, string program)
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
                f = shell.WaitForString("Meteroi shell>");			// program shell
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
            shell.VirtualScreen.CleanScreen(); 
            shell.SendResponse(command, true);	// send password 
            f = shell.WaitForString("Meteroi shell>");			   // program shell
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

    class command
    { 
        
    }

    class PCAS
    {
        static broad b = null;
        static Queue command = new Queue();

        static void command_thread()
        {
            while (true)
            {
                command.Dequeue();
                Console.WriteLine("do a command\n");
            }
        }

        static void Main(string[] args)
        {
            connect("10.235.6.197", "lophilo", "lab123");
            Thread th = new Thread(command_thread);
            th.Start();

            while (true)
            {
                Console.WriteLine(get_box_temperature());
                Console.WriteLine(get_box_moisture());
            }
        }
        public static bool connect(string broad_address, string login_name, string login_pwd)
        {
            b = new broad(broad_address, login_name, login_pwd, "~/test/thermal");
            if (b.is_connect())
                return true;
            else
                return false;
        }
//shell_cmd_func_t shell_cmd_func_list[] = {
//    {"help",      "Print Help Manual",                 cli_help},
//    {"temp",      "show the temperature",              show_temperature},
//    {"moist",     "show the moisture",                 show_moisture},
//    {"tempt",     "Set the target temperature",        set_temperature},
//    {"moistt",    "Set the target temperature",        set_moisture},
//    {"scope",     "set the coordinates of the micro scope",set_microscop_position},
//    {"manual",    "Manual regulate the micro scope",   manual_calibration},
//    {"x",         "regular x of micro scope",          microscop_x},
//    {"y",         "regular y of micro scope",          microscop_y},
//    {"z",         "regular z of micro scope",          microscop_z},
//    {"move",      "Move to the sample",                microscop_move},
//    {"ref",       "set the reference point of micro scope", microscop_ref},
//    {"syf",       "syringe run forward",               syringe_plus},
//    {"syb",       "syringe run back",                  syringe_minus},
//    {"led",       "LED light",                         led},
//    {"ut",        "Unit test of the system",           unit_test},
//    {NULL, NULL, NULL}
//};
        public static float get_box_temperature()
        {
            string regexStr = @"[-+]?\b(?:[0-9]*\.)?[0-9]+\b";
            string response;
            float resualt;
            response = b.send_command_get_response("temp 1");
            MatchCollection mc = Regex.Matches(response, regexStr);
            resualt = float.Parse(mc[1].Value);
            return resualt;
        }
        public static float get_box_moisture()
        {
            string regexStr = @"[-+]?\b(?:[0-9]*\.)?[0-9]+\b";
            string response;
            float resualt;
            response = b.send_command_get_response("moist 1");
            MatchCollection mc = Regex.Matches(response, regexStr);
            resualt = float.Parse(mc[1].Value);
            return resualt;
        }
        public float get_chip_temperature()
        {
            string regexStr = @"[-+]?\b(?:[0-9]*\.)?[0-9]+\b";
            string response;
            float resualt;
            response = b.send_command_get_response("temp 2");
            MatchCollection mc = Regex.Matches(response, regexStr);
            resualt = float.Parse(mc[1].Value);
            return resualt;
        }
        public float get_chip_moisture()
        {
            string regexStr = @"[-+]?\b(?:[0-9]*\.)?[0-9]+\b";
            string response;
            float resualt;
            response = b.send_command_get_response("moist 2");
            MatchCollection mc = Regex.Matches(response, regexStr);
            resualt = float.Parse(mc[1].Value);
            return 0;
        }
        public void set_target_temperature(float target)
        {
            string com = "tempt " + target.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void set_target_moisture(float target)
        {
            string com = "moistt " + target.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void microscope_x(int x)
        {
            string com = "x " + x.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void micoscope_y(int y)
        {
            string com = "y " + y.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void micoscope_z(int z)
        {
            string com = "z " + z.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void move_to_sample(uint i)
        {
            string com = "move " + i.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void syringe_plus(uint i)
        {
            string com = "syf " + i.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void syringe_minus(uint i)
        {
            string com = "syb " + i.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void set_led(uint pwm)
        {
            string com = "led " + pwm.ToString();
            b.send_command_get_response(com);
            return;
        }
        public void set_ref(uint i)
        {
            string com = "ref " + i.ToString();
            b.send_command_get_response(com);
            return;
        }
    }

}
