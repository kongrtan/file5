function getFieldValues(fields, name) {
  const field = fields.find(function (f) { return f.name === name; });
  if (!field) return [];
  const v = field.values;
  if (Array.isArray(v)) return v;
  if (v && typeof v.toArray === 'function') return v.toArray();
  return [];
}

const fields = context.dataFrame.fields;

const names = getFieldValues(fields, 'name');
const flooIds = getFieldValues(fields, 'floo_id');
const trs = getFieldValues(fields, 'tr');
const pros = getFieldValues(fields, 'pro');
const isconnecteds = getFieldValues(fields, 'isconnected');
const counts = getFieldValues(fields, 'count');

const groups = {};

for (let i = 0; i < names.length; i++) {
  const key = names[i] + '|' + flooIds[i];
  if (!groups[key]) {
    groups[key] = { name: names[i], floo_id: flooIds[i] };
  }
  groups[key][trs[i]] = {
    pro: pros[i],
    isconnected: isconnecteds[i],
    count: counts[i]
  };
}

return Object.values(groups);
