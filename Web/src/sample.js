let str = `  
{
  "endpoints": [
    "Point A",
    "Point B",
    "Point C"
  ],
  "messages": [
    {
      "from": 0,
      "to": 1,
      "at": "2023-03-07T18:22:01+00:00",
      "label": "SIP/2.0 200 I am alive!",
      "text": "SIP/2.0 200 I am alive! ?(?)? \\nVia: SIP/2.0/UDP 10.233.35.24:5060;rport=5060;branch=z9hG4bKPj026cd86b-12d9-496b-ae73-1bed2ce076e2;received=10.233.35.24"
    },
    {
      "from": 1,
      "to": 0,
      "at": "2023-03-07T18:22:01+00:00",
      "label": "SIP/2.0 200 OK",
      "text": "SIP/2.0 200 OK\\nVia: SIP/2.0/WSS poqj3rtuobio.invalid;branch=z9hG4bK4146775;rport=62644;received=172.30.130.192\\nTo: "
    },
    {
      "from": 1,
      "to": 2,
      "at": "2023-03-07T18:22:01+00:00",
      "label": "SIP/2.0 401 Unauthorized",
      "text": "SIP/2.0 401 Unauthorized\\nVia: SIP/2.0/WSS 90dol3ql9aig.invalid;branch=z9hG4bK8832882;rport=57403;received=172.30.190.91\\nTo: "
    },
    {
      "from": 2,
      "to": 0,
      "at": "2023-03-07T18:22:01+00:00",
      "label": "REGISTER sip:asterisk SIP/2.0",
      "text": "REGISTER sip:asterisk SIP/2.0\\nVia: SIP/2.0/WSS 90dol3ql9aig.invalid;branch=z9hG4bK8832882\\nMax-Forwards: 69\\nTo: <sip:6800306@asterisk>\\nFrom: \\"6800306\\" "
    }
  ]
}
`;

function sample() {
    return str;
}