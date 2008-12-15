# Simple HTTP example
#
# A simple example using the HTTP plugin that shows the retrieval of a
# single page via HTTP. The resulting page is written to a file.
#
# More complex HTTP scripts are best created with the TCPProxy.

from net.grinder.script import Test
from net.grinder.script.Grinder import grinder
from net.grinder.plugin.http import HTTPPluginControl, HTTPRequest
from HTTPClient import NVPair


control = HTTPPluginControl.getConnectionDefaults()
control.setProxyServer("localhost", 8888)
control.setFollowRedirects(0)

#new NVPair("Connection", "close")

#control.setDefaultHeaders(NVPair("Connection", "close"),))

#NVPair[] def_hdrs = { new NVPair("Connection", "close") }
#control.setDefaultHeaders(def_hdrs)


test1 = Test(1, "PresentationWebService")
request1 = test1.wrap(HTTPRequest())

#test2 = Test(2, "HeartBeat")
#request2 = test2.wrap(HTTPRequest())

class TestRunner:
	def __call__(self):
		#request2.GET("http://ednwl21182/PV/test.payload")
		request1.GET("http://ednwl21182/NewPVFrontEnd/PresentationService.ashx")
		
		

