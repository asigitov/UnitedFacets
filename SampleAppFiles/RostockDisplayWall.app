-np 1 --oversubscribe -x DISPLAY=:0.0 --prefix /your/path/to/openmpi_184 -x LD_LIBRARY_PATH -x PATH -host 192.168.50.1 your_app.x86_64
-np 1 --oversubscribe -x DISPLAY=:0.1 --prefix /your/path/to/openmpi_184 -x LD_LIBRARY_PATH -x PATH -host 192.168.50.1 your_app.x86_64
-np 1 --oversubscribe -x DISPLAY=:0.0 --prefix /your/path/to/openmpi_184 -x LD_LIBRARY_PATH -x PATH -host 192.168.50.2 your_app.x86_64
-np 1 --oversubscribe -x DISPLAY=:0.1 --prefix /your/path/to/openmpi_184 -x LD_LIBRARY_PATH -x PATH -host 192.168.50.2 your_app.x86_64
-np 1 --oversubscribe -x DISPLAY=:0.0 --prefix /your/path/to/openmpi_184 -x LD_LIBRARY_PATH -x PATH -host 192.168.50.3 your_app.x86_64
-np 1 --oversubscribe -x DISPLAY=:0.1 --prefix /your/path/to/openmpi_184 -x LD_LIBRARY_PATH -x PATH -host 192.168.50.3 your_app.x86_64