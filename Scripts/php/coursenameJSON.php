<?

#
# Example PHP server-side script for generating
# responses suitable for use with jquery-tokeninput
#

# Connect to the database
mysql_pconnect("schedgen.dlinkddns.com", "CoreDump", "Eecs582Team") or die("Could not connect");
mysql_select_db("CoreDump") or die("Could not select database");

# Perform the query
$query = sprintf("SELECT course as id, course as name from coursename WHERE course LIKE '%s%%' ORDER BY course ASC LIMIT 20", mysql_real_escape_string($_GET["q"]));
$arr = array();
$rs = mysql_query($query);

# Collect the results
while($obj = mysql_fetch_object($rs)) {
    $arr[] = $obj;
}

# JSON-encode the response
$json_response = json_encode($arr);

# Optionally: Wrap the response in a callback function for JSONP cross-domain support
if($_GET["callback"]) {
    $json_response = $_GET["callback"] . "(" . $json_response . ")";
}

# Return the response
echo $json_response;

?>